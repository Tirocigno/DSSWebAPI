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
        public int[] capacitiesLeft;
        private static string dataDirectory = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
        private static int MAX_ANNEALING_STEPS = 2000000; //WITH 1000 or MORE -> STACKOVERFLOW
        private static int COOLING_SCHEDULE_CONSTANT = 100;
        private static double TEMPERATURE_COOLING_CONSTANT = 0.9;

        public BasicHeu(GAPInstance gap)
        {
            GAP = gap;
            n = GAP.numcli;
            m = GAP.numserv;
            this.capacitiesLeft = (int[])GAP.cap.Clone();
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
            int[] capleft = this.capacitiesLeft;
            int ii;

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
            int[] capleft = this.capacitiesLeft;
            int[,] cost = GAP.cost;
            int[,] req = GAP.req;
            int z = 0;
            bool isImproved = true;

            int k = this.constructiveEurFirstSol();

            /*Calcolo la somma dei costi dei diversi client*/
            for (int j = 0; j < n; j++)
            {
                z += cost[sol[j], j];
            }

            if(z == k )
            {
                writeOnLog("[Opt1-0]: Ciclo for del calcolo di z inutile");
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
        public int simulatedAnnealing(double temperature, int steps, double decreaseConstant, int coolingScheduleSteps)
        {
            int z = this.constructiveEurFirstSol();
            sol = nonRecorsiveAnnealing(sol, temperature, 0, steps, decreaseConstant, coolingScheduleSteps, z);
            return checkSol(sol);
        }

        public int tabuSearch(int tabuQueueSize, int stepsToDo)
        {
            this.constructiveEurFirstSol();
            sol = computeTabuSearch(sol, tabuQueueSize, stepsToDo);
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
        }        // See this algorithm should not try an illegal configuration and should change capacities        private int[] nonRecorsiveAnnealing(int[] solution, double temperature, int step, int totalSteps, double temperatureDecrease, int coolingScheduleSteps, int initialCost)
        {
            int cost = initialCost;
            Random r = new Random();

            while (step < totalSteps)
            {
                int z = cost;
                int oldi;
                int randomServerIndex;
                int randomClientIndex;
                int treshold = 0;
                if (step % coolingScheduleSteps == 0)
                {
                    temperature *= temperatureDecrease;
                }

                int[] capleft= (int[])this.capacitiesLeft.Clone();
      
                //Fintanto che la nuova combinazione produce un assegnamento non possibile per colpa delle capacità

                do
                {
                    treshold++;
                    randomServerIndex = r.Next(m);
                    randomClientIndex = r.Next(n);
                    oldi = solution[randomClientIndex];
                    if(treshold == 1000)
                    {
                        return solution;
                    }
                } while (capleft[randomServerIndex] < GAP.req[randomServerIndex, randomClientIndex] ||
                oldi == randomServerIndex);
                treshold = 0;

                int[] tmpsol = (int[])solution.Clone();
                tmpsol[randomClientIndex] = randomServerIndex;
                capleft[randomServerIndex] -= GAP.req[randomServerIndex, randomClientIndex];
                capleft[oldi] += GAP.req[oldi, randomClientIndex];
                z -= (GAP.cost[oldi, randomClientIndex] - GAP.cost[randomServerIndex, randomClientIndex]);


                //int cost = checkSol(tmpsol);
                //int oldcost = checkSol(solution);
                if (z < cost)
                {
                    solution = (int[])tmpsol.Clone();
                    this.capacitiesLeft = (int[])capleft.Clone();
                    cost = z;
                } else
                {
                    double p = Math.Exp(-(z - cost) / temperature);
                    int rp = new Random().Next(0, 100);
                    if ( rp < p * 100)
                    {
                        solution = (int[])tmpsol.Clone();
                        this.capacitiesLeft = (int[])capleft.Clone();
                        cost = z;
                    }
                }
                step++;
            }

            return solution;
        }

        //COST FOR ELBA SHOULD BE 12302
        private int[] computeTabuSearch(int[] solution, int tabuQueueSize, int stepsToDo)
        {
            int bestAbsoluteSolution = checkSol(solution); //Best solution ever, used for accept Tabu Values.
            int bestLocalSolution = Int32.MaxValue; //Best local solution, used inside the algorithm;
            Queue<Pair<int, int>> tabuQueue= new Queue<Pair<int, int>>(tabuQueueSize); //Queue used for Tabu moves track
            int[] tmpsol = (int[])solution.Clone();
            int[] nextsol = (int[])solution.Clone();
            Pair<int, int> bestPosition = new Pair<int, int>(-1, -1);
            int step = 0;

            /*Stuff from Opt1-0*/

            int z = bestAbsoluteSolution;
            int isol = 0;
            int[] capleft = new int[m];
            int[] nextCapleft = new int[m];
            int[,] cost = GAP.cost;
            int[,] req = GAP.req;

            /*Check everything with local solution algorithm, take the best solution(even if is worse than the current) 
             * and set it as the current solution, add the tuple to tabu list and then go on */

            while(step < stepsToDo)
            {
                 bestLocalSolution = Int32.MaxValue; //Reset the local solution.

                /*Per ogni client*/
                for (int j = 0; j < n; j++)
                {
                    //writeOnLog("________NUOVO CLIENT________");

                    /*Per ogni server*/
                    for (int i = 0; i < m; i++)
                    {
                        //Setto le capacità iniziali a quelle attuali per ogni server
                        capleft = (int[])this.capacitiesLeft.Clone();
                        //Inizializzo la soluzione 
                        tmpsol = (int[])solution.Clone();
                        isol = tmpsol[j];
                        //Creo la tupla della posizione i server e j client.
                        Pair<int, int> currentPosition = new Pair<int, int>(i, j);
                        //Pongo il costo temporaneo uguale a z
                        int tempCost = z;
                        //Accetto soluzione anche se peggiorativa
                        if (i != isol && capleft[i] >= req[i, j])
                        {
                            tmpsol[j] = i;
                            capleft[i] -= req[i, j];
                            capleft[isol] += req[isol, j];
                            tempCost -= (cost[isol, j] - cost[i, j]);
                           // writeOnLog("[TS] Assegnare client" + j + " a server " + i + " cambia il costo a " + tempCost);

                            //Caso in cui la soluzione non è tabù ed è minore della soluzione migliore locale.
                            if (!tabuQueue.contains(currentPosition) && tempCost <= bestLocalSolution)
                            {
                                bestLocalSolution = tempCost;
                                nextsol = (int[])tmpsol.Clone();
                                bestPosition = currentPosition;
                                nextCapleft = (int[])capleft.Clone();
                                // Se la soluzione trovata è migliorativa, il suo costo è il nuovo best absolute solution.
                               // writeOnLog("[TS] Soluzione [" + i + ", " + j + "] accettata per più piccolo costo locale " + bestLocalSolution);
                                if (bestLocalSolution < bestAbsoluteSolution)
                                {
                                    bestAbsoluteSolution = bestLocalSolution;
                                   // writeOnLog("[TS] Soluzione [" + i + ", " + j + "] accettata per costo ottimo " + bestAbsoluteSolution);
                                }

                               

                            }
                            //Caso in cui la soluzione è tabu ma è migliore della soluzione più bella.
                            if (tabuQueue.contains(currentPosition) && tempCost <= bestLocalSolution
                                && tempCost < bestAbsoluteSolution)
                            {
                                bestLocalSolution = tempCost;
                                bestAbsoluteSolution = tempCost;
                                nextsol = (int[])tmpsol.Clone();
                                bestPosition = currentPosition;
                                nextCapleft = (int[])capleft.Clone();
                               // writeOnLog("[TS] Soluzione [" + i + ", " + j + "] accettata nonostante tabu");
                            }
                        }
                    }
                }

                //Assegno la nuova soluzione trovata
                solution = (int[])nextsol.Clone();
                // Aggiungo la posizione che l'ha generata alla tabu list.
                tabuQueue.add(bestPosition);
               
                //Assegno le nuove capacità sulla base della mossa che ho appena fatto
                this.capacitiesLeft = (int[])nextCapleft.Clone();
                //Il costo della soluzione che prendo in considerazione al prossimo passo sarà pari a bestLocalSolution.
                z = bestLocalSolution;
                writeOnLog("Tabu List: ");
                string tl = "[";
                for( int i = 0; i < tabuQueue.list.Count; i++)
                {
                    tl += "pos(" + tabuQueue.getValues().ElementAt(i).getFirstElem().ToString() + ", " +
                        tabuQueue.getValues().ElementAt(i).getSecondElem().ToString() + "), ";
                }
                writeOnLog(tl);
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