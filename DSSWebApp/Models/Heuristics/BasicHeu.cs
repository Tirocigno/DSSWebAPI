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
        double EPSILON = 0.01; 
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

            Console.WriteLine(sol);
           int z = checkSol(sol);
           GAP.zub = z;
           return z;

        }

        /*FINISH THIS LOCAL OPTIMUM*/
        public int opt10() //what cost should be?
        {
            int isol = 0;
            int[] capleft = GAP.cap;
            int[,] cost = GAP.cost;
            int[,] req = GAP.req;
            int z = 0; //What's z, what's initial number?
            bool isImproved = true;

            for(int j = 0; j < m; j++)
            {
                z += cost[sol[j], j];
            }

            while(isImproved) {
                isImproved = false;
                for (int j = 0; j < n; j++)
                {
                    isol = sol[j];

                    for (int i = 0; i < m; i++)
                    {
                        if (i != isol && cost[i, j] < cost[isol, j] && capleft[i] > req[isol, j])
                        {
                            isImproved = true;
                            sol[j] = i;
                            capleft[i] -= req[i, j];
                            capleft[isol] += req[i, j];
                            z -= (cost[isol, j] - cost[i, j]);
                            if (z < GAP.zub)
                            {
                                GAP.zub = z;
                                Console.WriteLine("[1-0opt]: new zub" + GAP.zub);
                            }
                        }
                    }
                }
            }

            double zCheck = 0;
            for (int j = 0; j < n; j++) zCheck += cost[sol[j], j];
            if(Math.Abs(z - zCheck) > EPSILON) // what's the value of EPSILON
            {
                Console.WriteLine("[1-0opt]:Not goood");
            }
            //WHAT TO DO NEXT?
            return z;    
        }

        public int simulatedAnnealing()
        {
            /*1. Genera una soluzione iniziale ammissibile S,
inizializza S* = S e il parametro temperatura T.
2. Genera S’∈N(S).
3. se z(S') < z(S) allora S=S', if (z(S*) > z(S)) S* = S
altrimenti accetta di porre S=S' con probabilità
p = e
-(z(S')-z(S))/kT
.
4. se (annealing condition) cala T.
5. se not(end condition) go to step 2.*/
            //CI
            int isol = 0;
            int[] capleft = GAP.cap;
            int[,] req = GAP.req;
            int[,] cost = GAP.cost;
            int z = 0; //What's z, what's initial number?
            bool isImproved = true;
            Random rnd = new Random(550);

            while (isImproved)
            {
                isImproved = false;
               // for (int j = 0; j < n; j++)
                {
                    int j = rnd.Next(0, m - 1);
                    isol = sol[j];

                    //for (int i = 0; i < m; i++)
                    {
                        int i = rnd.Next(0, n - 1);
                        if (i != isol && cost[i, j] < cost[isol, j] && capleft[i] > req[isol, j])
                        {
                            isImproved = true;
                            sol[j] = i;
                            capleft[i] -= req[i, j];
                            capleft[isol] += req[i, j];
                            z -= (cost[isol, j] - cost[i, j]);
                            if (z < GAP.zub)
                            {
                                GAP.zub = z;
                                Console.WriteLine("[1-0opt]: new zub" + GAP.zub);
                            }
                        }
                    }
                }
            }

            double zCheck = 0;
            for (int j = 0; j < n; j++) zCheck += cost[sol[j], j];
            if (Math.Abs(z - zCheck) > EPSILON) // what's the value of EPSILON
            {
                Console.WriteLine("[1-0opt]:Not goood");
            }
            //WHAT TO DO NEXT?
            return (int)z;
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