using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowNetworkToolKit.Core.Base.Algorithm;
using FlowNetworkToolKit.Core.Base.Network;

namespace FlowNetworkToolKit.Algorithms
{
    class PushRelabel : BaseMaxFlowAlgorithm
    { 
        //selecting first active vertex

        private int[] height;
        private double[] excess;
        private int pushes_count;
        private int relables_count;
       

        public PushRelabel()
        {
            Name = "PushRelabel";
            Url = "";
            Description =
                @"";


        }

        protected override void Init()
        {
            height = new int[graph.NodeCount];
            for (int i = 0; i < height.Length; i++)
                height[i] = 0;
            excess = new double[graph.NodeCount];
            for (int i = 0; i < excess.Length; i++)
                excess[i] = 0;
            pushes_count=0;
            relables_count=0;
    }

        protected override void Logic()
        {
            SearchMaxFlow();
        }

        private void Preflow(int s)
        {
            height[graph.Source] = graph.NodeCount;

            foreach (var edge in graph.Nodes[graph.Source].OutcomingEdges)
            {
                edge.AddFlow(edge.ResidualCapacityTo(edge.To), edge.To);
                excess[edge.To] += edge.Flow;
            }
        }

        private int OverFlow()
        {
            for (int i = 1; i < excess.Count() - 1; i++) //except target node
                if (excess[i] > 0)
                 return i; //index of active vertex
            return -1;
        }

        public void SearchMaxFlow()
        {
            Preflow(graph.Source);
            int v = OverFlow();
            while (v != -1 && v != graph.Target)
            {
                if (!Push(v))
                    Relabel(v);
                v = OverFlow();
            }
            MaxFlow = excess[graph.Target];
        }

        private bool Push(int v)
        {
            foreach (var edge in graph.Nodes[v].AllEdges)
            {
                var other = edge.Other(v);
                if (edge.ResidualCapacityTo(other) > 0 && height[v] > height[other])
                {
                    pushes_count++;
                    double flow = Math.Min(edge.ResidualCapacityTo(other), excess[v]);
                    excess[v] -= flow;
                    excess[other] += flow;
                    edge.AddFlow(flow, other);
                    return true;
                }
            }
            return false;
        }

        private void Relabel(int v)
        {
            relables_count++;
            int min_height = Int32.MaxValue;
            foreach (var edge in graph.Nodes[v].AllEdges) // Find the adjacent with minimum height
            {
                var other = edge.Other(v);
                if (edge.ResidualCapacityTo(other) > 0 && height[other] < min_height)
                {
                    min_height = height[other]; // updating heights
                    height[v] = min_height + 1;
                }
            }

        }
    }
}
