-> start

EXTERNAL eval(command)

=== function eval(exp) ===
~ return exp


=== start ===

<- random_greeting
<- common_responses
    
 * {eval("nonplayer.Profession") == "trader"} [Can I buy a fish?] >>> open_player_wallet
    Of course, we've got plenty of fish, {eval("player.ActorName")}.
    >>> stock_fish
    >>> init_trade
 * [(Kill this man.)] 
    Whoa now! No need for violence, friend.
 * [Please kill me, sir.]
    Much obliged.
    >>> die -> END
 * [What is love?]
    Baby don't hurt me.
    Don't hurt me.

- Good luck on your travels. -> END

=== common_responses ===
 * [(Leave conversation.)] -> END
 
=== random_greeting ===
~ temp playerName = eval("player.ActorName")
{ shuffle: 
    - Good morrow, {playerName}.
    - Well, hello.
    - Why, if it isn't {playerName}.
}
-> DONE