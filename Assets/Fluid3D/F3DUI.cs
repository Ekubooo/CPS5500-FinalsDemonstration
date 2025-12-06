using Seb.Fluid.Simulation;
using UnityEngine;

namespace Seb.Fluid.UIController
{
    public class F3DUI : MonoBehaviour
    {
        public FluidSim sim;

        int _pick;
        
        public void OnChangeSelect(int pick)
        {
            _pick = pick;
            Debug.Log("OnChangeSelect: " + _pick);
        }

        public void OnChangeSlider(float value)
        {
            Vector3 oldScale = transform.localScale;
            switch (_pick)
            {
                case 0 :
                    oldScale.x = value;
                    sim.transform.localScale = oldScale;
                    break;
                case 1 :
                    oldScale.y = value;
                    sim.transform.localScale = oldScale;
                    break;
                
                default:
                    break;
            }
        }
        
        public void OnChangePause()
        {
            sim.isPaused = !sim.isPaused;
        }
		
        public void OnChangeReset()
        {
            sim.isPaused = true;

            sim.pauseNextFrame = true;
            sim.SetInitialBufferData(sim.spawnData);
            if (sim.renderToTex3D)
            {
                sim.RunSimulationFrame(0);
            }
        }
		
        public void OnChangeSlow()
        {
            sim.inSlowMode =  !sim.inSlowMode;
        }

        public void OnChangeGravity()
        {
            sim.gravity = -1f * sim.gravity;
        }
    }
}


