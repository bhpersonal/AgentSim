using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgentSim1;
using System.Drawing;

namespace AgentSim1Tests
{
    [TestClass]
    public class SimTests
    {
        [TestMethod]
        public void Sim_3x6Creation_CorrectValues()
        {
            var sim = new Sim(new SimConfig { Width = 3, Height = 6 }, new Rules() { Goal = 2 });

            Assert.AreEqual(2, sim.Cells.GetUpperBound(0));
            Assert.AreEqual(5, sim.Cells.GetUpperBound(1));
            Assert.AreEqual(3, sim.Agents.Count);
            Assert.AreEqual(new Point(1, 1), sim.Agents[0].Location);
            Assert.AreEqual(new Point(1, 3), sim.Agents[1].Location);
            Assert.AreEqual(new Point(1, 5), sim.Agents[2].Location);

            Assert.AreEqual(sim.Cells[1, 1], CellState.Agent);
            Assert.AreEqual(sim.Cells[1, 3], CellState.Agent);
            Assert.AreEqual(sim.Cells[1, 5], CellState.Agent);
            
            Assert.AreEqual(2, sim.Rules.Goal);
        }

        [TestMethod]
        public void Sim_7x7Creation_CorrectAgents()
        {
            var sim = new Sim(new SimConfig { Width = 5, Height = 5 }, new Rules() { Goal = 2 });

            Assert.AreEqual(4, sim.Cells.GetUpperBound(0));
            Assert.AreEqual(4, sim.Cells.GetUpperBound(1));
            Assert.AreEqual(4, sim.Agents.Count);
            Assert.AreEqual(new Point(1, 1), sim.Agents[0].Location);
            Assert.AreEqual(new Point(1, 3), sim.Agents[1].Location);
            Assert.AreEqual(new Point(3, 1), sim.Agents[2].Location);
            Assert.AreEqual(new Point(3, 3), sim.Agents[3].Location);

            Assert.AreEqual(CellState.Agent, sim.Cells[1, 1]);
            Assert.AreEqual(CellState.Agent, sim.Cells[1, 3]);
            Assert.AreEqual(CellState.Agent, sim.Cells[3, 1]);
            Assert.AreEqual(CellState.Agent, sim.Cells[3, 3]);
        }

        [TestMethod]
        public void Sim_3x3Run_ActivatesAndLoops()
        {
            var sim = new Sim(new SimConfig { Width = 3, Height = 3 }, new Rules() { CellVisitOrder = new [] {0,1,2,3,4,5,6,7}, Goal = 4 });

            Assert.AreEqual(2, sim.Cells.GetUpperBound(0));
            Assert.AreEqual(2, sim.Cells.GetUpperBound(1));
            Assert.AreEqual(1, sim.Agents.Count);
            Assert.AreEqual(new Point(1, 1), sim.Agents[0].Location);

            Assert.AreEqual(CellState.Off, sim.Cells[0, 0]);
            sim.DoTurn();
            Assert.AreEqual(CellState.On, sim.Cells[0, 0]);

            Assert.AreEqual(CellState.Off, sim.Cells[1, 0]);
            sim.DoTurn();
            Assert.AreEqual(CellState.On, sim.Cells[1, 0]);

            Assert.AreEqual(CellState.Off, sim.Cells[2, 0]);
            sim.DoTurn();
            Assert.AreEqual(CellState.On, sim.Cells[2, 0]);

            Assert.AreEqual(CellState.Off, sim.Cells[2, 1]);
            sim.DoTurn();
            Assert.AreEqual(CellState.On, sim.Cells[2, 1]);

            Assert.AreEqual(CellState.Off, sim.Cells[2, 2]);
            sim.DoTurn();
            Assert.AreEqual(CellState.Off, sim.Cells[2, 2]);

            sim.DoTurn();
            sim.DoTurn();
            sim.DoTurn();

            Assert.AreEqual(0, sim.Agents[0].NextVisitIndex);
            sim.DoTurn();
            Assert.AreEqual(1, sim.Agents[0].NextVisitIndex);
        }

        [TestMethod]
        public void Sim_3x3Goal0_KillsCell()
        {
            var sim = new Sim(new SimConfig { Width = 3, Height = 3 }, new Rules() { CellVisitOrder = new[] { 0, 1, 2, 3, 4, 5, 6, 7 }, Goal = 0 });

            sim.Cells[2, 2] = CellState.On;

            sim.DoTurn();
            sim.DoTurn();
            sim.DoTurn();
            sim.DoTurn();

            Assert.AreEqual(CellState.On, sim.Cells[2, 2]);
            sim.DoTurn();
            Assert.AreEqual(CellState.Off, sim.Cells[2, 2]);
        }

        [TestMethod]
        public void Sim_3x3Goal3CustomOrder_CorrectCellsSet()
        {
            var sim = new Sim(new SimConfig { Width = 3, Height = 3 }, new Rules() { CellVisitOrder = new[] { 7, 3, 5, 1, 0, 2, 4, 6 }, Goal = 3 });

            sim.DoTurn();
            Assert.AreEqual(CellState.On, sim.Cells[0, 1]);

            sim.DoTurn();
            Assert.AreEqual(CellState.On, sim.Cells[2, 1]);

            sim.DoTurn();
            Assert.AreEqual(CellState.On, sim.Cells[1, 2]);

            sim.DoTurn();
            Assert.AreEqual(CellState.Off, sim.Cells[1, 0]);
        }

        [TestMethod]
        public void Sim_5x3MultipleAgents_FightingOverCell()
        {
            var sim = new Sim(new SimConfig { Width = 5, Height = 3 }, new Rules() { CellVisitOrder = new[] { 3, 7, 3, 7 }, Goal = 1 });

            sim.DoTurn();
            Assert.AreEqual(CellState.On, sim.Cells[2, 1]);
            Assert.AreEqual(CellState.On, sim.Cells[4, 1]);

            sim.DoTurn();
            Assert.AreEqual(CellState.Off, sim.Cells[0, 1]);
            Assert.AreEqual(CellState.Off, sim.Cells[2, 1]); // Agent #2 is disastified with cell being on.

            sim.DoTurn();
            Assert.AreEqual(CellState.On, sim.Cells[2, 1]); // Agent #1 wants cell turned on again
            Assert.AreEqual(CellState.On, sim.Cells[4, 1]);

        }
    }
}
