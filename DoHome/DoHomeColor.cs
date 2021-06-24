namespace DoHome
{
    using System;
    using System.Drawing;
    using System.Text.Json.Serialization;

    public class DoHomeColor
    {
        /// <summary>
        /// Red Gradient
        /// </summary>
        /// <remarks>
        /// Between 0 and 5000.
        /// </remarks>
        [JsonPropertyName("r")]
        public int Red { get; set; }

        /// <summary>
        /// Green Gradient
        /// </summary>
        /// <remarks>
        /// Between 0 and 5000.
        /// </remarks>
        [JsonPropertyName("g")]
        public int Green { get; set; }

        /// <summary>
        /// Blue Gradient
        /// </summary>
        /// <remarks>
        /// Between 0 and 5000.
        /// </remarks>
        [JsonPropertyName("b")]
        public int Blue { get; set; }

        /// <summary>
        /// White Gradient
        /// </summary>
        /// <remarks>
        /// Between 0 and 5000.
        /// </remarks>
        [JsonPropertyName("w")]
        public int White { get; set; }

        /// <summary>
        /// Warmth Gradient
        /// </summary>
        /// <remarks>
        /// Between 0 and 5000.
        /// </remarks>
        [JsonPropertyName("m")]
        public int Warmth { get; set; }

        public DoHomeColor()
        {

        }

        public DoHomeColor(int red, int green, int blue, int white, int warmth)
        {
            if (red is < 0 or > 5000) throw new ArgumentOutOfRangeException(nameof(red));
            if (green is < 0 or > 5000) throw new ArgumentOutOfRangeException(nameof(green));
            if (blue is < 0 or > 5000) throw new ArgumentOutOfRangeException(nameof(blue));
            if (white is < 0 or > 5000) throw new ArgumentOutOfRangeException(nameof(white));
            if (warmth is < 0 or > 5000) throw new ArgumentOutOfRangeException(nameof(warmth));
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.White = white;
            this.Warmth = warmth;
        }

        public DoHomeColor(Color color)
        {
            var brightness = color.GetBrightness() * 100;
            this.Red = Convert.ToInt32((50 * color.R / 255) * brightness);
            this.Green = Convert.ToInt32((50 * color.G / 255) * brightness);
            this.Blue = Convert.ToInt32((50 * color.B / 255) * brightness);
            this.White = 0;
            this.Warmth = 0;
        }
    }
}
