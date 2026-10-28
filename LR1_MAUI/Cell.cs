namespace LR1_MAUI
{
    public class Cell
    {
        public string Address { get; }
        public string Expression { get; set; } = "";
        public string Value { get; set; } = "";

        public Cell(string address)
        {
            Address = address;
        }
    }
}
