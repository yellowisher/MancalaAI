# MancalaAI
Yet another mancala AI (Minimax, MCTS) implementation.  
Even though [mancala is solved game](https://jabaier.sitios.ing.uc.cl/iic2622/kalah.pdf), it is still fun to play with those algorithms!  
You can play it [here](https://yapy.itch.io/mancalaai).

## Interesting things
### Minimax
I used simple `(myScore - opponentScore)` as heuristic.  
At first, I set high weight for winning state (like `1000 points when already scored more than half`), but this leads to *suicide moves* (giving up all the stones) if already winning / *boring moves* just to delay losing.
This looks fool at first glance (while reasonable though), so I stick to simple score.  

In mancala, if player finishes a move at his/her scoring pot, that player get an extra turn.  
Considering the depth of this extra turn just like a regular move might be unfair, because if so, some leaf nodes (that is estimated by heuristic) are in opponent's turn while some are mine.  
Of course, such moves will be favored.  

So I considered extra move as same depth as previous move. You might think it's still unfair because leaf nodes has different progression.  
But my heuristic is *pure relative*, so the game progression doesn't matter.

### MCTS
On 1vs1 game, MCTS is very similar to minimax. Both consider opponent as smart as itself, simulates worst actions for itself.  
But minimax is more strict. It is right for its depth. MCTS is more rough but deeper. (And yes, they have different and more important properties, like `aheuristic`, `anytime`)  
So it is make sense that there is some `Minimax-MCTS hybrid` approach, MCTS for big picture and minimax for details.

Best action selection at root node (after enough iteration of `selection-expand-simuate-back propagation`) is selecting *robust child* (child with most visit).
This isn't intuitive for me, and still I'm not sure about it. ([link](https://ai.stackexchange.com/questions/16905/mcts-how-to-choose-the-final-action-from-the-root) about it)

### Implementation
Not much interesting things here :p I used fixed array for stack-live board state, and removed threading for WebGL build.

## Experiments
| Matchup (first=start)  | Win    | Lose   | Draw   |
| ---------------------- | ------ | ------ | ------ |
| Random vs Random       | 47.4%  | 46.8%  |  5.8%  |
| Random vs Minimax      |  0.1%  | 99.7%  |  0.2%  |
| Random vs MCTS         |     -  | 99.9%  |  0.1%  |
| Minimax vs Random      |  100%  |     -  |     -  |
| Minimax vs Minimax     |  100%* |     -  |     -  |
| Minimax vs MCTS        |  100%  |     -  |     -  |
| MCTS vs Random         |  100%  |     -  |     -  |
| MCTS vs Minimax        | 27.9%  | 57.2%  | 14.9%  |
| MCTS vs MCTS           | 70.4%  | 21.9%  |  7.7%  |

\* My implementation of minimax doesn't contain any randomness.

I tested with depth 7 for minimax, 8000 iteration for MCTS. Each matchup is played 1000 times.  
