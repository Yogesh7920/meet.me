using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Controls;

namespace Client
{
    interface IWhiteBoardUpdater
    {
        abstract void FetchServerUpdates();
        abstract void RenderUXElement();
    }

    public class WhiteBoardViewModel : IWhiteBoardUpdater
    {

        ///UX sets this enum to different options when user clicks on the appropriate tool icon
        public enum WBTools
        {
            Initial, //Initialised value, never to be used again
            Selection,
            NewLine,
            NewRectangle,
            NewEllipse,
            Rotate,
            Move,
            Eraser
            //FreeHand
        };

        public List<int> selectedShapes;
        public WBTools ActiveTool;

        
       
        public WhiteBoardViewModel()
        {
            ActiveTool = WBTools.Initial;
            selectedShapes = new List<int>();
        }

        public void FetchServerUpdates()
        {
            throw new NotImplementedException();
        }

        public void RenderUXElement()
        {
            throw new NotImplementedException();
        }
    }
}
