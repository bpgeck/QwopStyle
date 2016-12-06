# QwopStyle
### Introduction
QWOP is a notoriously difficult flash game that requires the player press Q, W, O, and P to control a humanoid ragdoll's thighs and calves in an attempt to make the ragdoll run 100 meters. The original game can be found here: https://www.foddy.net/Athletics.html.
Continuing the research started by Steven Ray, Vahl Scott Gordon, Laurent Vaucher, http://research.google.com/pubs/pub42902.html, I attempt to train a computer to play the flash game, QWOP, in an optimal fashion using a cellular genetic algorithm.
Genetic algorithms are essentially optimization algorithms where every member of a population of strings is given a "fitness" score and is then mated with another string from this scored population to generate two new strings that will be members of the next generation's population.
This algorithm of generating a population, scoring the population, and mating viable candidates continues for many generations until, ultimately, the optimal solution is reached.

![QWOP Client](https://s15.postimg.org/vqujjx9u3/Capture.png "QWOP Client")

### Methodolgy
In the case of QwopStyle, each member of a population is a string of command characters as defined by encoding 2 from Ray, Gordon, and Vaucher's original paper:

"Encoding 2 encodes input sequences using a 16-character alphabet, each letter of which represents one of the possible input combinations in QWOP....Encoding 2 encodes
input sequences using a 16-character alphabet, each letter of which represents one of the possible input combinations in QWOP:"

![Encoding 2](https://s17.postimg.org/b6lbbk5xr/Capture.png)

"'FGBCHFELMIEFNGJCLHLEMCLJKJLNEKGHDGJDAJLE'  translates to “press Q and O, hold for 150ms, press P and release O while continuing to hold Q, wait for 150ms, release Q and O and press W, hold for 150ms, release W and press O, wait…” and so on."

In the initial paper, these commands are then sent to the executable running the QWOP client on loop for 60 seconds. After which **if the QWOP runner didn't fall**, the fitness score of the runner with this command string is the number of meters the runner progressed.
After all runners in the population have received their fitness scores each runner, **so long as this member has a fitness score higher than that of his parent runners** (in the case of the first population this is skipped since members of the initial population have no parents), is paired up with the best runner amongst its neighbors.
This locality of options was done by the initial researchers to "isolate by distance within the population, and promote niches of subpopulations that can improve diversity and help to prevent premature convergence.
After the mates are chosen, a "cut-and-splice" crossover algorithm (depicted below) is performed to generate the children strings.

![Cut-And-Splice](https://upload.wikimedia.org/wikipedia/en/7/73/CutSpliceCrossover.png)

These children strings then become the new population to be tested and teh cycle repeats

### Testing and Results
Due to the finicky nature of the QWOP physics engine, the exact same command string run consecutive times can result in different fitness values. To make matters worse, a once successful command string (the runner doesn't crash) can actually crash the next time it is run due the physics engine.
As a result, the defining factor in the success of the member was its **stability** (i.e. it ability to **not** fall over).
In addition, a runner's stability was usually determined within the first few commands executed in the string.

This discovery inspired me to seperate a runner's command string into two parts: An initialization sequence of commands run once at the beginning the trial, and a command sequence run repeatedly until the end of the 60 second trial.
I also assigned a fitness score to this initialization sequence of either 1 or 0 depending on whether or not the runner remained standing after the sequence.
Then, in the mate selection area of my algorithm, I mate initialization sequences independently of the command sequences.
This independence of mating resulted in the average fitness of the runners to converge to a maximum of 9 in-game meters per 1 real-time minute very quickly (often in 7 generations or less).
In addition, these initialization sequences usually converged to very few command characters (usually less than 6 characters) that simply spread the QWOP runner's legs very wide apart like so:

![Stable Stance](https://s17.postimg.org/qk6d1x0n3/Capture.png)

### Conclusion
Overall a very fun project to work on made even better by the fact that it actually worked (not a common occurance amongst my projects).
In general, I think that the finicky nature of the QWOP physics engine means that prioritizing not-crashing over distance the way I and the original researchers did, will cause the runners to converge to the sequences of highest stability.
To obtain that truly optimal "speed-running" time, I believe one would need to prioritize distance traveled by each member over all other factors.
This, however, will likely result in many crashes amongst the members of the first few generations, thus a very large population size for each generation must be used, and the more members, the longer each generation will take.
With parallel programming this is much more feasible, and I may revisit this project in the future to find out for sure if it is truly possible to optimize the playing of QWOP.
