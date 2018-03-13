using System.Collections;
using System.Collections.Generic;
using Utilities;
using System;
using UnityEngine;
using System.Threading;

public class MontecarloTree {

    ///factor = (valor / visitas) + 2 sqroot( ln(visitasPAdre) / visitas)

    private static float CONSTANT_TERM = 2f;


    /// <summary>
    /// The array that will store the tree
    /// Root is placed at [0]
    /// children = (parnt position) * 5 + (child number)
    /// parent = (child number - 1) / 5
    /// When it runs out of space, it will be resized to the double of its capacity
    /// </summary>
    private M_Node[] tree;

    /// <summary>
    /// The id of the player that will use this tree
    /// and which actions will be random
    /// </summary>
    private int id;

    /// <summary>
    /// This mutex will be locked every time an operation that changes nodes values
    /// is performed.
    /// </summary>
    private Mutex treeMutex;

    /// <summary>
    /// The flow controller we will use to simulate
    /// </summary>
    private M_FlowController flowController;


    /// <summary>
    /// Generates the root and the first expansion of nodes
    /// </summary>
    /// <param name="gameStateAtRoot">THe game that will be in the root</param>
    /// <param name="id">Player id of the owner of the tree</param>
    /// <param name="maxConcurrentTasks">Maximmum number of simultaneous simulations</param>
    public MontecarloTree(TGame gameStateAtRoot, int id)
    {
        Debug.Log("Creando arbol");

        tree = new M_Node[100];
        tree[0] = new M_Node(gameStateAtRoot, 0);
        this.id = id;
        flowController = new M_FlowController();
        treeMutex = new Mutex();
        ExtendNode(tree[0]);
    }

    /// <summary>
    /// Returns the best posible action found by the Montecarlo at the moment this method is called
    /// </summary>
    /// <returns>THe best posible action</returns>
    public Actions GetBestAction()
    {
        double max = double.NegativeInfinity;
        int currentBest = -1;
        for (int i = 0; i < GlobalData.NUMBER_OF_ACTIONS; i++)
        {
            if (tree[0 + i + 1].Score > max)
            {
                max = tree[0 + i + 1].Score;
                currentBest = i;
            }

        }
        return (Actions)currentBest;
    }



    /// <summary>
    /// Searches for the next node to explore. If there are already enough petitions waiting to be simulated,
    /// it does nothing
    /// </summary>
    public M_Node SearchForNextNode()
    {
        Debug.Log("Buscando mejor hijo");
        treeMutex.WaitOne();
        ///FALTA COMPROBAR PETICIONES ACUMULADAS

        //we start at the root
        M_Node currentNode = tree[0];

        Debug.Log("Empezamos en la raiz");
        //if the root has no children available, we wait
        if (currentNode.freeChildren <= 0)
        {
            Debug.Log("No hay nodos libres");
            treeMutex.ReleaseMutex();
            return null;
        }

        

        //while the node is not a leaf
        while (tree[currentNode.Position * 5 + 1] != null)
        {
            //if not, we start looking

            //we keep searching
            currentNode = tree[currentNode.Position * 5 + GetBestChild(currentNode)];

            //if the array needs to be resized
            if ((currentNode.Position * (5 + 1)) >= (tree.Length - 1))
            {
                Debug.Log("Se amplia el arbol");
                Array.Resize<M_Node>(ref tree, (tree.Length * 10));
            }
            Debug.Log("Pasamos por el nodo " + currentNode.Position);
        }

        //now we have a leaf node
        
        //if it has been visited, we expand it and get the best node
        if (currentNode.Visits > 0)
        {
            if (currentNode.State.SomeoneWon() != GlobalData.NO_PLAYER)
                return null;
            ExtendNode(currentNode);
            currentNode = tree[currentNode.Position * 5 +GetBestChild(currentNode)];
        }

        //we return the objective node
        //we return here no matter if the game has been won or not, to allow backpropagation
        BackpropagateFreeChildrenDecrement(currentNode);
        Debug.Log("El hijo que va a explorarse es " + currentNode.Position);
        currentNode.Visits++;
        currentNode.Available = false;
        treeMutex.ReleaseMutex();
        return currentNode;

    }

    /// <summary>
    /// Extends a node by creating a child fore each available action
    /// </summary>
    /// <param name="node">Parent node</param>
    private void ExtendNode(M_Node node)
    {
        Debug.Log("Expandiendo nodos");
        treeMutex.WaitOne();
        //security check

        //if the game in the node has already been won we don't extend it
        if (node.State.SomeoneWon() != GlobalData.NO_PLAYER)
        {
            Debug.Log("El nodo contiene victoria, no se expande");
            return;
        }

        //if the array already has children
        if (tree[node.Position * 5 + 1] != null)
        {
            Debug.LogError("EL NODO QUE SE HA EXPANDIDO YA TIENE HIJOS");
        }

        //we get the state in the node
        TGame initial = node.State;
        M_Node aux;

        //for each action we...
        for (int i = 0; i < GlobalData.NUMBER_OF_ACTIONS; i++)
        {
            ///...execute the action and advance the corresponding turns...
            flowController.AdvanceTurnAndExecuteActions(id, (Actions) i, initial);
            ///...create the new node with a copy of this state
            aux = new M_Node(initial.TakeAndGetSnapshot(), ((node.Position * 5) + i + 1));
            ///....restore the parent node state...
            initial.RestoreSnapshot();
            ///...set the new node as a child
            tree[aux.Position] = aux;

            //if in this node someone already won...
            if(aux.State.SomeoneWon() != GlobalData.NO_PLAYER)
            {
                //if our player won, reward
                if (aux.State.SomeoneWon() == id)
                    aux.Score += GlobalData.MONTECARLO_REWARD;
                else
                    aux.Score += GlobalData.MONTECARLO_PENALIZATION;
                //backpropagate score
                BackpropagateScore(aux);

                //we mark th node as not available, so it wont be visited again
                aux.Available = false;
                node.freeChildren--;

            }
            Debug.Log("Se ha creado el nodo " + aux.Position);
        }
        treeMutex.ReleaseMutex();
    }

    /// <summary>
    /// Takes the node with the new score and backpropagates it
    /// </summary>
    /// <param name="initial">The node from which the backpropagation will start WITH THE SCORE ON IT</param>
    public void BackpropagateScore(M_Node initial)
    {
        treeMutex.WaitOne();
        M_Node currentNode;
        int numberToPropagate = initial.Score;
        currentNode = tree[(initial.Position - 1) / 5];
        Debug.Log("Backpropagation");
        //until we reach the root
        while (currentNode.Position != 0)
        {
            Debug.Log("Se pasa por el nodo " + currentNode.Position + " que tenia " + currentNode.Score + " puntos");
            currentNode.Score += numberToPropagate;
            Debug.Log("Ahora tiene " + currentNode.Score + " puntos");
            currentNode = tree[(currentNode.Position - 1) / 5];
        }

        //to add to the root
        Debug.Log("Se pasa por el nodo " + currentNode.Position + " que tenia " + currentNode.Score + " puntos");
        currentNode.Score += numberToPropagate;
        Debug.Log("Ahora tiene " + currentNode.Score + " puntos");
        BackpropagateFreeChildrenIncrement(initial);
        initial.Available = true;
        treeMutex.ReleaseMutex();
    }

    /// <summary>
    /// Returns the index of the available child with the best posible factor value
    /// The parent's visits number will be increased here
    /// HAS TO BE CALLED WHEN THE MUTEX IS ALREADY LOCKED
    /// </summary>
    /// <returns>Index of the child (0-4)</returns>
    private int GetBestChild(M_Node parent)
    {
        #if UNITY_EDITOR
        if(parent.freeChildren <= 0)
            Debug.LogError("THE PARENT THAT WAS CHOSEN HAD NOT FREE CHILDREN");
        if(tree[parent.Position * 5] == null)
            Debug.LogError("THE PARENT THAT WAS CHOSEN HAD NOT CHILDREN");
        #endif

        double currentMax = double.NegativeInfinity;
        int winnerChild = -1;
        M_Node currentChild;
        double aux;
        Debug.Log("Buscando mejor hijo del nodo " + parent.Position);
        for (int i = 0; i < GlobalData.NUMBER_OF_ACTIONS; i++)
        {
            //get the child
            currentChild = tree[(parent.Position * 5) + i + 1];
            Debug.Log("Mirando el hijo " + currentChild.Position);
            ///if there are no free children or it is not available, the node is not considered
            if (currentChild.freeChildren <= 0 || !currentChild.Available)
            {
                Debug.Log("Todos los hijos estaban ocupados o el nodo no estaba libre");
                continue;
            }

            //if the node has not been visited, the value is infinite
            if (currentChild.Visits <= 0)
            {
                Debug.Log("El hijo no habia sido visitado y se selecciona");
                //since in case of a tie we visit the nodes in order and this value
                //cant be surpased, we stop searching and return this node
                parent.Visits++;
                return i + 1;
            }

            aux = (currentChild.Score / currentChild.Visits) + (CONSTANT_TERM * Math.Sqrt(Math.Log(parent.Visits) / currentChild.Visits));
            Debug.Log("punt = " + currentChild.Score + "/" + currentChild.Visits + " * 2 * raiz( ln(" + (parent.Visits) + ") / " + currentChild.Visits);
            Debug.Log("Obtiene una puntuacion de " + aux);

            if (aux > currentMax)
            {
                Debug.Log("Este es el nuevo mejor hijo");
                currentMax = aux;
                winnerChild = i;
            }
        }

        #if UNITY_EDITOR
        if (winnerChild == -1)
        {
            Debug.LogError("ALGO HA IDO MAL AL SACAR EL MEJOR HIJO Y NO SE HA ELEGIDO NINGUNO");
            return -2;
        }
        
        #endif

        parent.Visits++;
        Debug.Log("El mejor hijo es " + (parent.Position * 5 + winnerChild + 1) + " y el padre ahora tiene " + parent.Visits + " visitas");
        Debug.Log("Se visita el hijo " + tree[(parent.Position * 5) + winnerChild + 1].Position + " que ahora tiene " + tree[(parent.Position * 5) + winnerChild + 1].Visits + " visitas");
        return winnerChild + 1;

    }

    /// <summary>
    /// Decrements the number of free children in the parent and continues
    /// until a node has at least one free child
    /// </summary>
    private void BackpropagateFreeChildrenDecrement(M_Node start)
    {
        tree[(start.Position - 1) / 5].freeChildren--;
        Debug.Log("El nodo " + ((start.Position - 1) / 5) + " tiene ahora " + tree[(start.Position - 1) / 5].freeChildren + " hijos libres");
        if (tree[(start.Position - 1) / 5].freeChildren <= 0)
            BackpropagateFreeChildrenDecrement(tree[(start.Position - 1) / 5]);
    }

    private void BackpropagateFreeChildrenIncrement(M_Node start)
    {
        tree[(start.Position - 1) / 5].freeChildren++;
        Debug.Log("El nodo " + ((start.Position - 1) / 5) + " tiene ahora " + tree[(start.Position - 1) / 5].freeChildren + " hijos libres");
        //if before was zero, the grandparent now has a new free child
        if (tree[(start.Position - 1) / 5].freeChildren == 1)
            BackpropagateFreeChildrenIncrement(tree[(start.Position - 1) / 5]);
    }

}
