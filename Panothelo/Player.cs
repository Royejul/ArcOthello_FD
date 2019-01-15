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
        public int ID { get; }
        public ImageBrush ImagePawn { get; set; }
        public int Score { get; set; }

        public Player(String name, int id, ImageBrush image)
        {
            this.Name = name;
            this.ID = id;
            this.ImagePawn = image;
            Score = 2;

        }

        public override string ToString()
        {
            return Name;
        }
    }
}
