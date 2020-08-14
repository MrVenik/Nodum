namespace NodumVisualCalculator.Data
{
    public interface IVisualNode
    {
        bool Focused { get; set; }
        VisualNodeHolder Holder { get; }
        bool MenuShowed { get; set; }
        string Name { get; set; }
        Position Position { get; set; }
        bool Showed { get; set; }
        void Close();
    }
}