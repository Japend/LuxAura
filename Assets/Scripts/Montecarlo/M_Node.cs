public class M_Node
{

    public int Visits;
    public int Score;
    public TGame State;
    public M_Node[] Children;

    public M_Node(TGame state)
    {
        State = state;
        Visits = Score = 0;
        Children = new M_Node[GlobalData.NUMBER_OF_ACTIONS];
    }

}
