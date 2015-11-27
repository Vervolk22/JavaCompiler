# JavaCompiler
By Zholudzeu A.A. BSUIR, 4 term, 1st half-year.

Parses input program in Java language to token flow, then to syntax tree, makes semantic analysis and generates .exe file with CIL commands.

1. Can analyse + - * operations, int type, possible double type, while loop, if statement with < > == !=.
2. Program must have method, named "main". That is it's start point.
3. Semantic analysis can only check for duplicate identifiers, and assignment types coincidence.
4. Can only generate 1 class with 1 method (missing directives to make calls).
