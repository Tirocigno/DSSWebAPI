using System;
using System.Collections.Generic;
using System.IO;
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
        private static string dataDirectory = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
        private static int MAX_ANNEALING_STEPS = 2000000; //WITH 1000 or MORE -> STACKOVERFLOW
        private static int COOLING_SCHEDULE_CONSTANT = 100;
        private static double TEMPERATURE_COOLING_CONSTANT = 0.9;

        public BasicHeu(GAPInstance gap)
        {
            GAP = gap;
            n = GAP.numcli;
            m = GAP.numserv;

            writeOnLog("Numero di client: " + n);
            writeOnLog("Numero di server: " + m);
        }

        /*Construct a random solution.*/
        public int constructFirstSol()
        {
            sol = new int[n];
            for (int j = 0; j < n; j++)
            {
                sol[j] = j;
                writeOnLog("Client [" + j + "] assegnato a server [" + j + "]");
            }
              
            int z = checkSol(sol);
            return z;
        }

        public int constructiveEurFirstSol()
        {
            sol = new int[n];
            int[] keys = new int[m];
            int[] index = new int[m];
            int[] capleft = new int[m];
            int ii;

            for(int i = 0; i < m; i++)
            {
                capleft[i] = GAP.cap[i];
            }

            for (int j = 0; j < n; j++)
            {
                writeOnLog("[Constructive] Assegnamento di Client " + j);
                /*Per ogni client vado a salvare in keys[i] il costo tra i e j e metto in index l'indice del server*/
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
                    writeOnLog("[Constructive] Server esaminato " + i);
                    //Se la capacità del server i è sufficente -> allora diminuisco la capacità del server e vado ad assegnare il client a quel server
                    if (capleft[i] >= GAP.req[i, j])
                    {
                        writeOnLog("[Constructive]Client " + j + " viene assegnato a server " + i);
                        capleft[i] -= GAP.req[i, j];
                        writeOnLog("[Constructive]Capacità server " + i + " rimane " + capleft[i]);
                        sol[j] = i;
                        break;
                    } else
                    {
                        writeOnLog("[Constructive] Server esaminato " + i + "non poteva ospitare client " + j);
                    }
                   
                }
                if (ii == m)
                {
                    //Non ho trovato nessun server idoneo
                    return -1;
                }
            } 
           int z = checkSol(sol);
           GAP.zub = z;
           return z;

        }

        /*THIS IS WORKING*/
        public int opt10()
        {
            int isol = 0;
            int[] capleft = new int[m];
            int[,] cost = GAP.cost;
            int[,] req = GAP.req;
            int z = 0;
            bool isImproved = true;

            this.constructiveEurFirstSol();

            /*Inizializzo le capacità per ogni server*/
            for (int i = 0; i < m; i++)
            {
                capleft[i] = GAP.cap[i];
            }

            /*Calcolo la somma dei costi dei diversi client*/
            for (int j = 0; j < n; j++)
            {
                z += cost[sol[j], j];
            }


            while (isImproved) {
                isImproved = false;
                /*Per ogni client*/
                for (int j = 0; j < n; j++)
                {

                    /*Per ogni server*/
                    for (int i = 0; i < m; i++)
                    {
                        isol = sol[j];
                        if (i != isol && cost[i, j] < cost[isol, j] && capleft[i] >= req[i, j])
                        {
                            isImproved = true;
                            sol[j] = i;
                            capleft[i] -= req[i, j];
                            capleft[isol] += req[isol, j];
                            z -= (cost[isol, j] - cost[i, j]);
                            if (z < GAP.zub)
                            {
                                GAP.zub = z;
                            }
                        }
                    }
                }
            }

           

            double zCheck = 0;
            for (int j = 0; j < n; j++)
            {
                zCheck += cost[sol[j], j];
            }

            if (Math.Abs(z - zCheck) > EPSILON)
            {
                writeOnLog("Solution is different of: "+ Math.Abs(z - zCheck) + " should not be that!");
                return -1;
            }
            return z;    
        }

        /*Execute simulated annealing*/
        public int simulatedAnnealing(double temperature)
        {
            this.constructiveEurFirstSol();
            //sol = annealing(sol, temperature, 0);
            sol = nonRecorsiveAnnealing(sol, temperature, 0);
            return checkSol(sol);
        }

        // Check the solution cost.
        private int checkSol(int[] sol)
        {
            int z = 0, j;
            int[] capused = new int[m];
            for (int i = 0; i < m; i++) capused[i] = 0;
            // controllo assegnamenti
            for (j = 0; j < n; j++)
                if (sol[j] < 0 || sol[j] >= m)
                {
                    writeOnLog("Client " + j + " assegnato a server che non esiste");
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
                    writeOnLog("[checkSol] Cap disponibile " + GAP.cap[sol[j]]);
                    writeOnLog("[checkSol] Cap utilizzata " + capused[sol[j]]);
                    writeOnLog("[checkSol] Server " + sol[j] + " sfora nella capacita");
                    z = Int32.MaxValue;
                    return z;
                } else
                {
                   // writeOnLog("[checkSol] Cap utilizzata aumentata a  " + capused[sol[j]]);
                }
            }
            return z;
        }

        /*1. Genera una soluzione iniziale ammissibile S,
           inizializza S* = S e il parametro temperatura T.
          2. Genera S’∈N(S).
          3. se z(S') < z(S) allora S=S', if (z(S*) > z(S)) S* = S
             altrimenti accetta di porre S=S' con probabilità p = e-(z(S')-z(S))/kT.
          4. se (annealing condition) cala T.
          5. se not(end condition) go to step 2.          */
        private int[] annealing(int[] solution, double temperature, int step)
        {
            System.Threading.Thread.Sleep(100);
            writeOnLog("Lunghezza di sol al passo " + step +" risulta: " + solution.Length.ToString());
            if(step % COOLING_SCHEDULE_CONSTANT == 0)
            {
                temperature *= TEMPERATURE_COOLING_CONSTANT;
            }
            if(step == MAX_ANNEALING_STEPS)
            {
                return solution;
            }
            int randomServerIndex = new Random().Next(m);
            int randomClientIndex = new Random().Next(n);

            int[] tmpsol = (int[])sol.Clone();
            tmpsol[randomClientIndex] = randomServerIndex;

           
                int cost = checkSol(tmpsol);
                int oldcost = checkSol(solution);
                if (cost < oldcost)
                {
                    solution = null;
                    return annealing(tmpsol, temperature, step + 1);
                }
                double p = Math.Exp(-(cost - oldcost) / (double)temperature);
                if (new Random().Next() < p)
                {
                    solution = null;
                    return annealing(tmpsol, temperature, step + 1);
                }
                    
            
            tmpsol = null;
            return annealing(solution, temperature, step + 1);
        }        private int[] nonRecorsiveAnnealing(int[] solution, double temperature, int step)
        {
            while(step < MAX_ANNEALING_STEPS)
            {
                if (step % COOLING_SCHEDULE_CONSTANT == 0)
                {
                    temperature *= TEMPERATURE_COOLING_CONSTANT;
                }
               
                int randomServerIndex = new Random().Next(m);
                int randomClientIndex = new Random().Next(n);

                int[] tmpsol = (int[])sol.Clone();
                tmpsol[randomClientIndex] = randomServerIndex;

                int cost = checkSol(tmpsol);
                int oldcost = checkSol(solution);
                if (cost < oldcost)
                {
                    solution = tmpsol;
                }
                double p = Math.Exp(-(cost - oldcost) / (double)temperature);
                if (new Random().Next() < p)
                {
                    solution = tmpsol;
                }

                step++;
            }

            return solution;
        }        private void writeOnLog(string message)
        {
            StreamWriter writeLog = new StreamWriter(dataDirectory + "\\log.txt", true);
            string messageToWrite = "[" + DateTime.Now.ToString() + "]: " + message;
            writeLog.WriteLine(message);
            writeLog.Close();
        }
    }
}