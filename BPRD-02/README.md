# BPRD-02

The second assignment in Programmer som data. In this README, all none code answers are given.

## PLC Exercises

### 2.4 & 2.5

Answered in `Intcomp1.fs`.

### 3.2

regex: `^(a?)(b|ba)*$`

<figure>
    <img src="img/PLC_3(2)_NFA.jpg"
         alt="3(2) NFA">
    <figcaption>Exercise 3.2 NFA.</figcaption>
</figure>

<br><br>

<figure>
    <img src="img/PLC_3(2)_DFA.jpg"
         alt="3(2) DFA">
    <figcaption>Exercise 3.2 DFA.</figcaption>
</figure>

## BCD Exercises

### 2.1

Here, we have assumed that the number 42 is a part or not a part of the number-string. Thus, the following regexes scan to find occurences of the number 42.

- a: `^(\d*(42)).*$`
- b: `^(?!\d*(42)).*$`
- c: `^([4-9][3-9]|\d{3,})$`

### 2.2

<figure>
    <img src="img/BCD_2(2)_NFA.jpg"
         alt="2(2) NFA">
    <figcaption>Exercise 2.2 NFA.</figcaption>
</figure>

<br><br>

<figure>
    <img src="img/BCD_2(2)_DFA.jpg"
         alt="2(2) DFA">
    <figcaption>Exercise 2.2 DFA.</figcaption>
</figure>

## HelloLex Exercises

### Question 1

*What are the regular expressions involved, and which semantic values are they associated with?*

`[0-9]`: all positive integers

### Question 2

*Generate the lexer out of the specification using a command prompt. Which additional file is generated during the process?*

`hello.fs`

*How many states are there by the automaton of the lexer?*

3

### Question 3

*Compile and run the generated program `hello.fs` from question 2.*

`fsharpc` does not work for us.

### Question 4

*Extend the lexer specification `hello.fsl` to recognize numbers of more than one digit. New lexer specification is `hello2.fsl`. Generate `hello2.fs`, compile and run the generated program.*

Not finished.

### Question 5

*Extend the lexer specification `hello2.fsl` to recognize floating numbers. New lexer specification is hello3.fsl. Generate `hello3.fs`, compile and run the generated program.*

Not finished.
