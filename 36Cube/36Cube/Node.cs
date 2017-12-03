using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _36Cube
{
    
    public class Node
    {
        public Node Parent = null;
        public bPoint Location = new bPoint(-1, -1);
        public Block Block = null;

        public List<Node> Children = new List<Node>();

        public bool isSolution { get { return Location.X + 1 == Utility.Heights.Length && Location.Y + 1 == Utility.Heights.Length; } }
        
        public Node() { }

        public Node(Node Parent, Block AddedBlock)
        {
            this.Parent = Parent;
            this.Location = Utility.GetNextLocation(Parent);
            this.Block = AddedBlock;
        }

        public void Dispose()
        {
            Parent = null;
            Location = null;
            Block = null;
            Children = null;
        }
    }
}
