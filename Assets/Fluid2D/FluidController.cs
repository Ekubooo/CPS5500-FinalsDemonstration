using Seb.Fluid2D.Simulation;
using UnityEngine;

namespace Seb.Fluid2D.UIController
{
    public class FluidController : MonoBehaviour
    {
        public FluidSim2D sim;
        
        public void OnChangeSizeX(float width)
        {
            // move barrier, do not pause!
            sim.isPaused = false;
            sim.obstacleSize.x = width;
        }

        public void OnChangeSizeY(float height)
        {
            // move barrier, do not pause!
            sim.isPaused = false;
            sim.obstacleSize.y = height;
        }

        public void OnChangeLocationX(float x_axis)
        {
            // move barrier, do not pause!
            sim.isPaused = false;
            sim.obstacleCentre.x = x_axis;
        }

        public void OnChangeLocationY(float y_axis)
        {
            // move barrier, do not pause!
            sim.isPaused = false;
            sim.obstacleCentre.y = y_axis;
        }
		
        public void OnChangeBoundX(float W)
        {
            // move barrier, do not pause!
            sim.isPaused = false;
            sim.boundsSize.x = W;
        }
		
        public void OnChangePause()
        {
            sim.isPaused = !sim.isPaused;
        }
		
        public void OnChangeReset()
        {
            sim.isPaused = true;
			
            sim.SetInitialBufferData(sim.spawnData);
            sim.RunSimulationStep();
            sim.SetInitialBufferData(sim.spawnData);
        }
		
        public void OnChangeStep()
        {
            sim.isPaused = false;
            sim.pauseNextFrame = true;
        }
    }
}


