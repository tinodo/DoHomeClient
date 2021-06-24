//-----------------------------------------------------------------------
// <copyright file="DoHomeColor.cs" company="Company">
//    Copyright (c) Tino Donderwinkel. All rights reserved.
// </copyright>
// <author>Tino Donderwinkel</author>
//-----------------------------------------------------------------------

namespace DoHome
{
    using System;
    using System.Drawing;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Class that presents a color on a <see cref="DoHomeDevice"/>.
    /// </summary>
    public class DoHomeColor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoHomeColor"/> class.
        /// </summary>
        /// <param name="red">Red gradient</param>
        /// <param name="green">Green gradient</param>
        /// <param name="blue">Blue gradient</param>
        /// <param name="white">White gradient</param>
        /// <param name="warmth">Warm gradient</param>
        /// <remarks>
        /// Gradients should have a value between 0 and 5000.
        /// </remarks>
        public DoHomeColor(int red, int green, int blue, int white, int warmth)
        {
            if (red is < 0 or > 5000)
            {
                throw new ArgumentOutOfRangeException(nameof(red));
            }

            if (green is < 0 or > 5000)
            {
                throw new ArgumentOutOfRangeException(nameof(green));
            }

            if (blue is < 0 or > 5000)
            {
                throw new ArgumentOutOfRangeException(nameof(blue));
            }

            if (white is < 0 or > 5000)
            {
                throw new ArgumentOutOfRangeException(nameof(white));
            }

            if (warmth is < 0 or > 5000)
            {
                throw new ArgumentOutOfRangeException(nameof(warmth));
            }

            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.White = white;
            this.Warmth = warmth;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoHomeColor"/> class based on a <see cref="Color"/>.
        /// </summary>
        /// <param name="color">A <see cref="Color"/>.</param>
        public DoHomeColor(Color color)
        {
            var brightness = color.GetBrightness() * 100;
            this.Red = Convert.ToInt32((50 * color.R / 255) * brightness);
            this.Green = Convert.ToInt32((50 * color.G / 255) * brightness);
            this.Blue = Convert.ToInt32((50 * color.B / 255) * brightness);
            this.White = 0;
            this.Warmth = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoHomeColor"/> class.
        /// </summary>
        internal DoHomeColor()
        {
        }

        /// <summary>
        /// Gets or sets the red gradient
        /// </summary>
        /// <remarks>
        /// Between 0 and 5000.
        /// </remarks>
        [JsonPropertyName("r")]
        public int Red { get; set; }

        /// <summary>
        /// Gets or sets the green gradient
        /// </summary>
        /// <remarks>
        /// Between 0 and 5000.
        /// </remarks>
        [JsonPropertyName("g")]
        public int Green { get; set; }

        /// <summary>
        /// Gets or sets the blue gradient
        /// </summary>
        /// <remarks>
        /// Between 0 and 5000.
        /// </remarks>
        [JsonPropertyName("b")]
        public int Blue { get; set; }

        /// <summary>
        /// Gets or sets the white gradient
        /// </summary>
        /// <remarks>
        /// Between 0 and 5000.
        /// </remarks>
        [JsonPropertyName("w")]
        public int White { get; set; }

        /// <summary>
        /// Gets or sets the warmth gradient
        /// </summary>
        /// <remarks>
        /// Between 0 and 5000.
        /// </remarks>
        [JsonPropertyName("m")]
        public int Warmth { get; set; }
    }
}
