namespace IconGenerator
{
    public class Icon
    {
        public string InputPath { get; }

        public string OutputPath { get; }

        public int Width { get; }

        public int Height { get; }

        public bool MaintainAspectRatio { get; }

        public Icon(string input, string outputPath, int width, int height, bool maintainAspectRatio)
        {
            InputPath = input;
            OutputPath = outputPath;
            Width = width;
            Height = height;
            MaintainAspectRatio = maintainAspectRatio;
        }

        public void Generate()
        {
            ImageUtilities.ResizeImage(InputPath, OutputPath, Width, Height, MaintainAspectRatio);
        }

        public override string ToString()
        {
            return $"{InputPath} -> {OutputPath} ({Width}x{Height})";
        }
    }
}