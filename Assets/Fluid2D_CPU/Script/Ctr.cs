using PBA.Fluid2D.Main;
using UnityEngine;

namespace PBA.Fluid2D.UICtr
{
    public class Crt : MonoBehaviour
    {
        public Main.Fluid2D _sim;
        
        public void OnChangeBondX(float width)
        {
           _sim.boundSize.x = width;
        }

        public void OnChangeBondY(float height)
        {
            _sim.boundSize.y = height;
        }
    }
}