-> start

EXTERNAL eval(command)

=== function eval(exp) ===
~ return 404


=== start ===

~ temp profession = eval("nonplayer.Profession")
~ temp using_workstation = eval("nonplayer.Obj.UsingWorkstation")

{profession == "trader": -> is_trader}

{profession == "banker" && using_workstation : -> is_banker}

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


=== is_banker ===

<- random_greeting
-> help_player

= help_player
{<> How can I help you?|Will there be anything else?}
 + [What's the balance of my debt?]
    {Hmm, let me see...|<>}
    {
    - eval("player.CurrentDebt") <= 0:
        {You don't appear to have any debt!|You...still don't have any.}
    - else:
        Your current debts total $<player.CurrentDebt>.
    }
    -> help_player
 + {eval("player.CurrentDebt") > 0}[I'd like to make a payment.]
    {Excellent. |}You currently owe $<player.CurrentDebt>. How much will you be paying {today|this time}?
    ~ temp balance = eval("player.Wallet.Balance")
    ~ temp debt = eval("player.CurrentDebt")
    ~ temp payment = 0
    + + {balance > 0 && debt > 0}[A dollar.]
        Really? {One dollar?|Again?}
        ~ payment = 1
    + + {balance >= 10 && debt >= 10}[Ten dollars.]
        I see. You're sure?
        ~ payment = 10
    + + {balance >= 100 && debt >= 100}[One hundred dollars.]
        Excellent. You're sure?
        ~ payment = 100
    + + {balance >= 1000 && debt >= 1000}[One thousand dollars.]
        Excellent. You're sure?
        ~ payment = 1000
    + + {debt > 0 && balance >= debt}[All of my remaining debt.]
        All of it? You're certain?
        ~ payment = debt
    + + [Never mind.{balance == 0: (You have no money.)}] -> help_player
    - -
    + + [I'm sure. (Pay {payment} dollar{payment != 1:s|}.)]
        >>> pay_debt {payment}
        It's done, then.
        {
        - debt > payment: 
            <> You now owe {debt - payment} dollar{debt - payment != 1:s|}.
        - else: 
            <> Your debt is paid in entirety.
        }
    + + [Never mind.]
    - - -> help_player
 * [{Never mind|That's all}. (Leave.)] -> END


=== common_exit_option ===

 * [(Leave conversation.)] -> END
 
 
=== random_greeting ===

{ shuffle: 
    - Hello there.
    - Well, hello.
    - Hello.
}
-> DONE





=== function print_num(x) ===
{
    - x >= 1000:
        {print_num(x / 1000)} thousand { x mod 1000 > 0:{print_num(x mod 1000)}}
    - x >= 100:
        {print_num(x / 100)} hundred { x mod 100 > 0:and {print_num(x mod 100)}}
    - x == 0:
        zero
    - else:
        { x >= 20:
            { x / 10:
                - 2: twenty
                - 3: thirty
                - 4: forty
                - 5: fifty
                - 6: sixty
                - 7: seventy
                - 8: eighty
                - 9: ninety
            }
            { x mod 10 > 0:<>-<>}
        }
        { x < 10 || x > 20:
            { x mod 10:
                - 1: one
                - 2: two
                - 3: three
                - 4: four
                - 5: five
                - 6: six
                - 7: seven
                - 8: eight
                - 9: nine
            }
        - else:
            { x:
                - 10: ten
                - 11: eleven
                - 12: twelve
                - 13: thirteen
                - 14: fourteen
                - 15: fifteen
                - 16: sixteen
                - 17: seventeen
                - 18: eighteen
                - 19: nineteen
            }
        }
}
