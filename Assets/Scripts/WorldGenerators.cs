using System;
using System.Collections.Generic;

public static class WorldGenerators
{
    static readonly System.Random rng = new System.Random();
    static int R(int a, int b) => rng.Next(a, b + 1);
    static T Pick<T>(IList<T> l) => l[rng.Next(l.Count)];

    // Index 0 = World 1 (Variables), 1 = World 2 (If/Else), etc.
    public static readonly Func<Question>[][] All = {
        new Func<Question>[] { Sum, Concat, TypeCheck },
        new Func<Question>[] { Comparison, Boolean, Conditional },
        new Func<Question>[] { ListIndex, ListLength, Append },
        new Func<Question>[] { RangeCount, LoopLast, LoopSyntax },
        new Func<Question>[] { DebugValue, Equality, PrintValue },
    };

    static Question Sum()
    {
        int a = R(2, 9), b = R(2, 9);
        return Question.Make("PREDICT", "What does this print?",
            $"a = {a}\nb = {b}\nprint(a + b)",
            (a + b).ToString(),
            new[] { (a * b).ToString(), (a + b + 1).ToString(), (a + b - 1).ToString(), (a - b).ToString() },
            "+ adds two numbers.");
    }

    static Question Concat()
    {
        string x = Pick(new[] { "cyber", "neo", "byte" }), y = Pick(new[] { "punk", "code", "leak" });
        return Question.Make("PREDICT", "What does this print?",
            $"s = \"{x}\"\nprint(s + \"{y}\")",
            x + y,
            new[] { x + " " + y, y + x, x + "+" + y },
            "+ joins strings with no space between them.");
    }

    static Question TypeCheck()
    {
        var v = Pick(new[] { ("42", "int"), ("3.14", "float"), ("\"Neo\"", "str"), ("True", "bool") });
        return Question.Make("TYPE CHECK", "What type is x?",
            $"x = {v.Item1}\nprint(type(x))", v.Item2,
            new[] { "int", "float", "str", "bool" },
            "Whole number = int, decimal = float, text in quotes = str, True/False = bool.");
    }

    static Question Comparison()
    {
        int n = R(2, 8);
        return Question.Make("CONDITIONS", "Which operator checks whether values are equal?",
            $"if power __ {n}:\n    open_gate()", "==", new[] { "=", "!=", ">" },
            "Use == to compare two values; = assigns a value.");
    }

    static Question Boolean() => Question.Make("CONDITIONS", "What is True and False called?",
        "powered = True\nlocked = False", "bool", new[] { "str", "int", "float" },
        "Boolean values represent either True or False.");

    static Question Conditional() => Question.Make("CONDITIONS", "Which branch runs when the if condition is false?",
        "if power > 50:\n    open_gate()\n__:\n    recharge()", "else", new[] { "elif", "then", "for" },
        "else handles the case when the if condition is false.");

    static Question ListIndex() => Question.Make("LISTS", "What does inventory[0] access?",
        "inventory = [\"battery\", \"chip\"]\nprint(inventory[0])", "battery",
        new[] { "chip", "0", "2" }, "Python lists start counting at index 0.");

    static Question ListLength() => Question.Make("LISTS", "How many items are in this list?",
        "parts = [\"wire\", \"chip\", \"gear\"]\nprint(len(parts))", "3",
        new[] { "2", "4", "0" }, "len(list) returns the number of items.");

    static Question Append() => Question.Make("LISTS", "Which method adds an item to the end of a list?",
        "inventory = [\"battery\"]\ninventory.____(\"chip\")", "append",
        new[] { "add", "insertEnd", "push" }, "append(value) adds a value at the end.");

    static Question RangeCount() => Question.Make("LOOPS", "How many times does this loop run?",
        "for i in range(5):\n    hit()", "5", new[] { "4", "6", "1" },
        "range(5) produces 0, 1, 2, 3, and 4.");

    static Question LoopLast() => Question.Make("LOOPS", "What is the last value of i?",
        "for i in range(10):\n    pass", "9", new[] { "10", "8", "0" },
        "range(10) stops before 10, so the final value is 9.");

    static Question LoopSyntax() => Question.Make("LOOPS", "What must end a Python for-loop header?",
        "for i in range(3)__", ":", new[] { ";", ")", "." },
        "Python uses a colon before an indented loop body.");

    static Question DebugValue() => Question.Make("DEBUG", "What does print(2 + 3) display?",
        "print(2 + 3)", "5", new[] { "23", "2 + 3", "6" }, "The + operator adds integers.");

    static Question Equality() => Question.Make("DEBUG", "Which operator means 'not equal to'?",
        "if status __ \"ready\":", "!=", new[] { "=", "==", "!" },
        "!= tests whether two values are different.");

    static Question PrintValue() => Question.Make("DEBUG", "What does this print?",
        "power = 100\nprint(power)", "100", new[] { "power", "0", "True" },
        "print(power) displays the value stored in power.");
}
