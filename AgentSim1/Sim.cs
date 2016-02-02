using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentSim1
{
    public static class General
    {
        public static Point[] CellOffsets = new Point [] { 
            new Point( -1, -1 ),
            new Point(  0, -1 ),
            new Point(  1, -1 ),
            new Point(  1,  0 ),
            new Point(  1,  1 ),
            new Point(  0,  1 ),
            new Point( -1,  1 ),
            new Point( -1,  0 )};
    }

    public static class SimFactory
    {
        public static Sim CreateRandomSim()
        {
            var rules = new Rules
            {
                CellVisitOrder = new int[] { 4, 1, 5, 7, 0, 2, 3, 6 },
                // CellVisitOrder = new int[] { 0, 2, 4, 6, 1, 3, 5, 7 },
               // CellVisitOrder = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 },
                Goal = 6,
            };

            //CellVisitOrder = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, + 7 = very cool
                
            var config = new SimConfig
            {
                Width = 101,
                Height = 101,
            };

            var sim = new Sim(config, rules);


            var r = new System.Random();


            var i = 0;

            foreach (var agent in sim.Agents)
            {
                //agent.Goal = r.Next(2,4);
                agent.NextVisitIndex = i % 8;
                i++;
            }

            for (var x = 0; x < sim.Cells.GetLength(0); x++ )
            {
                for (var y = 0; y < sim.Cells.GetLength(1); y++)
                {
                    if (sim.Cells[x, y] == CellState.Agent) continue;

                    sim.Cells[x, y] = r.Next(100) == 0 ? CellState.On : CellState.Off;
                }
            }

            return sim;
        }
    }

    public enum CellState
    {
        Off,
        On,        
        Agent,
    }

    public class Sim
    {
        public CellState[,] Cells;

        public int[,] CellsLastChange;

        public Rules Rules;

        public List<Agent> Agents;

        public int Turn;

        public Sim(SimConfig config, Rules rules)
        {
            Cells = new CellState[config.Width, config.Height];
            CellsLastChange = new int[config.Width, config.Height];

            Rules = rules;
            Agents = new List<Agent>();

            for (var agentX = 1; agentX < config.Width; agentX += 2)
            {
                for (var agentY = 1; agentY < config.Height; agentY += 2)
                {
                    var agent = new Agent
                    {
                        Sim = this,
                        Location = new Point(agentX, agentY),
                        Goal = rules.Goal,
                        CellVisitOrder = rules.CellVisitOrder,
                    };
                    Agents.Add(agent);

                    Cells[agentX, agentY] = CellState.Agent;
                }
            }
        }

        public void DoTurn()
        {
            foreach (var agent in Agents)
            {
                agent.DoTurn();
            }

            Turn++;
        }
    }

    public class SimConfig
    {
        public int Width;
        
        public int Height;
    }

    public class Rules
    {
        public int[] CellVisitOrder;

        public int Goal;
    }

    public class Agent
    {
        public Sim Sim;

        public Point Location;

        public CellState[] Beliefs = new CellState[8];

        public int Goal;

        public int[] CellVisitOrder;

        public int NextVisitIndex;

        public int DistanceFromGoal;

        public void DoTurn()
        {
            var observedCellIndex = CellVisitOrder[NextVisitIndex];
            var observedCellX = Location.X + General.CellOffsets[observedCellIndex].X;
            var observedCellY = Location.Y + General.CellOffsets[observedCellIndex].Y;

            if (observedCellX < Sim.Cells.GetLength(0) && observedCellY < Sim.Cells.GetLength(1))
            {

                // Observe & update belief
                var observedCell = Sim.Cells[observedCellX, observedCellY];
                Beliefs[observedCellIndex] = observedCell;

                var beliefCount = Beliefs.Count(x => x == CellState.On);

                // Beliefs are always true!
                beliefCount = General.CellOffsets.Select(x => Sim.Cells[Location.X + x.X, Location.Y + x.Y]).Count(x => x == CellState.On);

                DistanceFromGoal =
                    beliefCount < Goal ? 1 :
                    beliefCount > Goal ? -1 :
                    0;

                var isChanged = false;
                var newState = observedCell;
                if (DistanceFromGoal > 0 && observedCell == CellState.Off)
                {
                    newState = CellState.On;
                    isChanged = true;
                }
                else if (DistanceFromGoal < 0 && observedCell == CellState.On)
                {
                    newState = CellState.Off;
                    isChanged = true;
                }

                if (isChanged)
                {
                    Sim.Cells[observedCellX, observedCellY] = newState;
                    Beliefs[observedCellIndex] = newState;
                    Sim.CellsLastChange[observedCellX, observedCellY] = Sim.Turn;
                }
            }

            NextVisitIndex = (NextVisitIndex + 1) % 8;
        }
    }
}
