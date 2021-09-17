# Jammo.ParserTools

Nuget: https://www.nuget.org/packages/Jammo.ParserTools/1.1.2

A library of tools I frequently use when parsing text

## Tokenizer

### Constructors:

`Tokenizer: (new) Tokenizer(input, [options])`, `IEnumerable<BasicToken>: Tokenizer.Tokenize(input, [options])`

---------------------------

A take on IEnumerable which allows for manual .Next calls (moves the iterator forward)
The tokenizer class provides BasicToken(s) based on input data

## StateMachine

### Constructors:

`StateMachine: (new) StateMachine<Enum>()`

---------------------------

## IParserStream

Meant to be used alongside a parser to wrap a FileStream for easy Read/Write using fields. 
Exposes IsInitialized, FilePath, Parse, Write, and WriteTo.
