-> start

EXTERNAL eval(command)

=== function eval(exp) ===
~ return exp


=== start ===

~ temp profession = eval("nonplayer.Profession")

{profession == "trader": -> is_trader}

<- random_greeting

{profession != "": I work as a {profession}.}
    
<- common_exit_option

- Good luck on your travels. -> END



=== is_trader ===

<- random_greeting
Are you looking to trade?
 * [Sure.]
    >>> init_trade <nonplayer.ActorId>
    -> END
 * [No thanks.]
    -> END


=== common_exit_option ===

 * [(Leave conversation.)] -> END
 
 
=== random_greeting ===

~ temp playerName = eval("player.ActorName")
{ shuffle: 
    - Good morrow, {playerName}.
    - Well, hello.
    - Why, if it isn't {playerName}.
}
-> DONE

