public class M_Node
{

    public int Visits;
    public int Score;
    public TGame State;
    private int position;
    public int Position { get { return position; } }
    public int freeChildren;
    public bool Available;

    public M_Node(TGame state, int position)
    {
        State = state;
        Visits = Score = 0;
        freeChildren = GlobalData.NUMBER_OF_ACTIONS;
        this.position = position;
        Available = true;
    }

}
