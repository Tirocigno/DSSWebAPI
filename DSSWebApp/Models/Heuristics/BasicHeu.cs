using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSSWebApp.Models.Heuristics
{
    //Basic heuristic implementation, inside methods algorithms.
    class BasicHeu
    {
        GAPInstance GAP;
        int n, m;
        public int[] sol;

        public BasicHeu(GAPInstance gap)
        {
            GAP = gap;
            n = GAP.numcli;
            m = GAP.numserv;
        }

        /*Construct a random solution.*/
        public int constructFirstSol()
        {
            sol = new int[n];
            for (int j = 0; j < n; j++)
                sol[j] = j;
            int z = checkSol(sol);
            return z;
        }

        public int constructiveEurFirstSol()
        {
            sol = new int[n];
            int[] keys = new int[m];
            int[] index = new int[m];
            int[] capleft = GAP.cap;
            int ii;

            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < m; i++)
                {
                    keys[i] = GAP.req[i, j];
                    index[i] = i;
                }
                Array.Sort(keys, index);
                // Ciclo finché non trovo una capacità adatta 
                for (ii = 0; ii < m; ii++)
                {
                    int i = index[ii];
                    //Se la capacità del server i è sufficente -> allora diminuisco la capacità del server e vado ad assegnare il client a quel server
                    if (capleft[i] >= GAP.req[i, j])
                    {
                        capleft[i] -= GAP.req[i, j];
                        sol[j] = i;
                        break;
                    }
                }
                if (ii == m)
                {
                    //Non ho trovato nessun server idoneo
                    return -1;
                }
            }

            return checkSol(sol);
        }
        // Check the solution cost.
        public int checkSol(int[] sol)
        {
            int z = 0, j;
            int[] capused = new int[m];
            for (int i = 0; i < m; i++) capused[i] = 0;
            // controllo assegnamenti
            for (j = 0; j < n; j++)
                if (sol[j] < 0 || sol[j] >= m)
                {
                    z = Int32.MaxValue;
                    return z;
                }
                else
                    z += GAP.cost[sol[j], j];
            // controllo capacità
            for (j = 0; j < n; j++)
            {
                capused[sol[j]] += GAP.req[sol[j], j];
                if (capused[sol[j]] > GAP.cap[sol[j]])
                {
                    z = Int32.MaxValue;
                    return z;
                }
            }
            return z;
        }
    }
}