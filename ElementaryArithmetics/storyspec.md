
===============================
Story format specification [v1]
===============================
A story specifies the problem type, the operation and the description of the story in JSON form.<br>
The stories.json file provides some examples<br>
<p>

The DescriptionFormat must be formed correctly according to the operation and the problem type and must use the following replace variables for the operands and total:<br>
$LO$ = Left operand<br>
$RO$ = Right operand<br>
$TO$ = Total<br>
<p>
A Type = FindTotal, Operator = Addition story description looks like:<br>
"DescriptionFormat":  "John has $LO$ apples, but then he buys $RO$ more apples. How many apples does he have now?"<br>
<br>
A Type = FindLeftOperand, Operator = Subtraction story description looks like:<br>
"DescriptionFormat":  "John has some apples, but then he gives $RO$ to Jean and now John has $TO$ apples left. How many apples did John had at the beginning?"<br>
<br>
A Type = FindRightOperand, Operator = Multiplication story description looks like:<br>
"DescriptionFormat":  "$LO$ friends get together to play, between all of them they have $TO$ marbles. If each of them brought the same amount, how many marbles did each friend bring?<br>
<p>
===========
Tokens [v1]
===========
The DescriptionFormat can use optional tokens to create stories using different persons, animals, foods or things while using a fixed problem structure.<br>
\#PA1# = A randomly picked first name.<br>
\#AA1# = A randomly picked animal name.<br>
\#FA1# = A randomly picked food name.<br>
\#TA1# = A randomly picked thing name.<br>
<p>
The first letter of the identified represents the type:<br>
P = Person<br>
A = Animal<br>
F = Food<br>
T = Thing<br>
<br>
The second letter of the identifier represents the genre:<br>
A = Any<br>
M = Male<br>
F = Female<br>
<br>
The numbers after the 2 letters represent a contextual id to know if the description requires distinct values:<br>
<p>
The value of #PA1# will be the same for the entire story description. once the value of #PA1# is picked, all references to #PA1# on that story description will be the same.<br>
A story can have #PA1#, #PA2# to force different values.<br>
<br>
Example:<br>
"DescriptionFormat":  "#PA1# has $LO$ #FA1#, but then #PA1# gives $RO$ #FA1# to #PA2#. How many #FA1# does #PA1# have now?"<br>
<br>
\#PA1# and #PA2# will be replaced by randomly picked person names of any genre, and #FA# will be replease by a random food name. Such as:<br>
"DescriptionFormat":  "John has $LO$ apple, but then John gives $RO$ apple to Jean. How many apple does John have now?"<br>
<br>
Note that #FA1# got repleaced with a singular food name, which might or might not make sense depending on the values of the operands and total. Or the context of the story.<br>
To make that better, the description supports qualifiers for the tokens.<br>
<p>
=================
TOKEN QUALIFIERS
=================
The DescriptionFormat can use qualifiers to indicate if a singular or plural name should be used or if "he/she" should be used instead of a name.<br>
\#PA1qg# = replace with "he" or "she" depending on the genre of PA1.<br>
\#FA1qcPL# = replace with the plural name of the food identified by FA1.<br>
\#FA1qcSI# = replace with the singular name of the food identified by FA1.<br>
\#FA1qcLO# = replace with singular or plural name of the FA1 food, depending on the number identified by the left operand $LO$.<br>
\#FA1qcRO# = replace with singular or plural name of the FA1 food, depending on the number identified by the right operand $RO$.<br>
\#FA1qcTO# = replace with singular or plural name of the FA1 food, depending on the number identified by the total $TO$.<br>
<br>
Example:<br>
"DescriptionFormat":  "#PA1# has $LO$ #FA1qcLO#, but then #PA1qg# gives $RO$ #FA1qcRO# to #PA2#. How many #FA1qcPL# does #PA1# have left?"<br>
<br>
Would tranform to something like:<br>
"DescriptionFormat" : "John has 5 bananas, but then he gives 1 banana to Jean. How many bananas does John have left?"<br>
