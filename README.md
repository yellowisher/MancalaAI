# MancalaAI
Yet another mancala AI (Minimax, MCTS) implementation.  
Even though [mancala is solved game](https://jabaier.sitios.ing.uc.cl/iic2622/kalah.pdf), it is still interesting environment for learning those algorithms.  
You can play it [here](https://yapy.itch.io/mancalaai).

## Interesting things
### Minimax
I used simple `(myScore - opponentScore)` as heuristic.  
At first, I set high weight for winning state (like `1000 points when already scored more than half`), but this leads to *suicide moves* (giving up all the stones) if already winning / *boring moves* just to delay losing.
This wasn't looks intuitive at first glance.  

In mancala, if your move ended at your scoring pot, you get an extra turn.  
Considering this extra turn as regular depth might be unfair, because if so, some leaf nodes (that is estimated by heuristic) are in opponent's turn while some are mine.  
Most of case states that are result of my action will be favored.  

So I considered extra move as same depth as previous move, which you might think it's still unfair because leaf nodes has different progression.  
But my heuristic is *pure relative*, so the game progression doesn't matter.

### MCTS
On 1vs1 game, MCTS is very similar to minimax. Both consider opponent as smart as itself, simulate with worst actions for itself.  
But minimax is more strict. It is right for their depth. MCTS is more rough but deeper. And there are some Minimax-MCTS hybrid approach, MCTS for big picture, minimax for details.  

### Implementation
Not much interesting things here :p I used fixed array for stack-live board state, and removed threading for WebGL build.

## Experiments
| Matchup (first=start)  | Win    | Lose   | Draw   |
| ---------------------- | ------ | ------ | ------ |
| Random vs Random       |     %  |     %  |     %  |
| Random vs Minimax      |     %  |     %  |     %  |
| Random vs MCTS         |     %  |     %  |     %  |
| Minimax vs Random      |     %  |     %  |     %  |
| Minimax vs Minimax     |     %* |     %  |     %  |
| Minimax vs MCTS        |     %  |     %  |     %  |
| MCTS vs Random         |     %  |     %  |     %  |
| MCTS vs Minimax        |     %  |     %  |     %  |
| MCTS vs MCTS           |     %  |     %  |     %  |

\* My implementation of minimax doesn't contain any randomness.

I tested with depth 7 for minimax, 8000 iteration for MCTS. Each matchup is played 1000 times. (still running!)
