
=============
Objectives
=============
1) Generate story style math problems given a configuration that regulates the types of arithmetic operations and the numbers to be used.
2) Generate simple equation problems that help kids with basic arithmetics.

=========================
Story configuration
=========================
1) Types of arithmetic operation (addition, subtraction, multiplication, division).
2) Maximum total to be used.
3) Nature of problem (missing operand, missing total).
4) A basic set of stories is store and loaded from stories.json
5) A basic set of story elements (used for replace tokens) is stored and loaded from storyelements.json


===============================
Story format specification [v1]
===============================
A story specifies the problem type, the operation and the description of the story with the form:
    {
      "Version": 1,
      "Type": 2,          // FindRightOperand = 0, FindLeftOperand = 1, FindTotal = 2.
      "Operator": 2,      // Addition = 0, Subtraction = 1, Multiplication = 2, Division = 3.
      "DescriptionFormat": "There was a little dog that needed some food."
    },

The DescriptionFormat must be formed correctly according to the operation and the problem type and must use the following replace variables for the operands and total:
$LO$ = Left operand
$RO$ = Right operand
$TO$ = Total

A Type = FindTotal, Operator = Addition story description looks like:
"DescriptionFormat":  "John has $LO$ apples, but then he buys $RO$ more apples. How many apples does he have now?"

A Type = FindLeftOperand, Operator = Subtraction story description looks like:
"DescriptionFormat":  "John has some apples, but then he gives $RO$ to Jean and now John has $TO$ apples left. How many apples did John had at the beginning?"

A Type = FindRightOperand, Operator = Multiplication story description looks like:
"DescriptionFormat":  "$LO$ friends get together to play, between all of them they have $TO$ marbles. If each of them brought the same amount, how many marbles did each friend bring?

TOKENS
------

The DescriptionFormat can use optional tokens to create stories using different persons, animals, foods or things while using a fixed problem structure.
#PA1# = A randomly picked first name.
#AA1# = A randomly picked animal name.
#FA1# = A randomly picked food name.
#TA1# = A randomly picked thing name.

The first letter of the identified represents the type:
P = Person
A = Animal
F = Food
T = Thing

The second letter of the identifier represents the genre:
A = Any
M = Male
F = Female

The numbers after the 2 letters represent a contextual id to know if the description requires distinct values:
The value of #PA1# will be the same for the entire story description. once the value of #PA1# is picked, all references to #PA1# on that story description will be the same.
A story can have #PA1#, #PA2# to force different values.

Example:
"DescriptionFormat":  "#PA1# has $LO$ #FA1#, but then #PA1# gives $RO$ #FA1# to #PA2#. How many #FA1# does #PA1# have now?"

#PA1# and #PA2# will be replaced by randomly picked person names of any genre, and #FA# will be replease by a random food name. Such as:
"DescriptionFormat":  "John has $LO$ apple, but then John gives $RO$ apple to Jean. How many apple does John have now?"

Note that #FA1# got repleaced with a singular food name, which might or might not make sense depending on the values of the operands and total. Or the context of the story.
To make that better, the description supports qualifiers for the tokens.

TOKEN QUALIFIERS
----------------

The DescriptionFormat can use qualifiers to indicate if a singular or plural name should be used or if "he/she" should be used instead of a name.
#PA1qg# = replace with "he" or "she" depending on the genre of PA1.
#FA1qcPL# = replace with the plural name of the food identified by FA1.
#FA1qcSI# = replace with the singular name of the food identified by FA1.
#FA1qcLO# = replace with singular or plural name of the FA1 food, depending on the number identified by the left operand $LO$.
#FA1qcRO# = replace with singular or plural name of the FA1 food, depending on the number identified by the right operand $RO$.
#FA1qcTO# = replace with singular or plural name of the FA1 food, depending on the number identified by the total $TO$.

Example:
"DescriptionFormat":  "#PA1# has $LO$ #FA1qcLO#, but then #PA1qg# gives $RO$ #FA1qcRO# to #PA2#. How many #FA1qcPL# does #PA1# have left?"

Would tranform to something like:
"DescriptionFormat" : "John has 5 bananas, but then he gives 1 banana to Jean. How many bananas does John have left?"


