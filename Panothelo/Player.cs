using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Panothelo
{
    public class Player
    {
        public String Name { get; }
        public ImageBrush ImagePawn { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="image"></param>
        public Player(String name, ImageBrush image)
        {
            this.Name = name;
            this.ImagePawn = image;
        }
    }
}
