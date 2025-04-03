using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

class Program
{
    static void Main()
    {
        string imagePath = "input.png";
        int scaleFactor = 2;
        bool invert = true;

        using (var image = Image.Load<Rgba32>(imagePath))
        {
            int outputWidth = (image.Width / 2) / scaleFactor;
            int outputHeight = (image.Height / 4) / scaleFactor;

            image.Mutate(x => x
                .Resize(outputWidth * 2, outputHeight * 4)
                .Grayscale()
                .DetectEdges()
                .Contrast(1.5f)
                .BinaryDither(KnownDitherings.Stucki)
            );

            using (var sw = new StreamWriter("braille_output.txt"))
            {
                for (int y = 0; y < outputHeight; y++)
                {
                    for (int x = 0; x < outputWidth; x++)
                    {
                        int brailleChar = 0x2800;
                        for (int dy = 0; dy < 4; dy++)
                        {
                            for (int dx = 0; dx < 2; dx++)
                            {
                                int px = x * 2 + dx;
                                int py = y * 4 + dy;
                                var pixel = image[px, py];

                                float brightness = pixel.R / 255f;
                                bool isDot = invert ? (brightness > 0.5f) : (brightness < 0.5f);

                                if (isDot)
                                {
                                    int dotIndex = dy * 2 + dx;
                                    brailleChar |= (1 << dotIndex);
                                }
                            }
                        }
                        Console.Write((char)brailleChar);
                        sw.Write((char)brailleChar);
                    }
                    Console.WriteLine();
                    sw.WriteLine();
                }
            }
        }
    }
}

