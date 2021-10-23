namespace GUI
{
    public interface ITooltipTrigger
    {
        public bool DoShowTooltip { get; }
        public string GetText();
    }
}