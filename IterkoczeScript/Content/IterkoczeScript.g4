grammar IterkoczeScript;

program: line* EOF;
line: statement | ifBlock | forBlock | whileBlock | foreachBlock | functionDefinition;

statement: (assingment | functionCall | returnStatement) ';';

useStatement: 'use' IDENTIFIER;

returnStatement: 'return' expression;

ifBlock: IF expression block ('else' elseIfBlock)?;

forBlock: 'for' assingment ';' expression ';' INTEGER ';' block;

foreachBlock: 'foreach' IDENTIFIER 'in' expression block;

elseIfBlock: block | ifBlock;

whileBlock: WHILE expression block ('else' elseIfBlock);

WHILE: 'while';

IF: 'if';

functionDefinition: DEFINE IDENTIFIER block;

functionCall: IDENTIFIER '(' (expression (',' expression)*)? ')';

DEFINE: 'def' | 'define';

assingment: IDENTIFIER '=' expression;

expression
    : constant                              #constantExp
    | '$' INTEGER                           #argumentIdentifierExp
    | returnStatement                       #returnStatementExp
    | IDENTIFIER                            #identifierExp
    | functionCall                          #functionCallExp
    | functionDefinition                    #functionDefinitionExp
    | '(' expression ')'                    #parenthesizedExp
    | '!' expression                        #notExp
    | expression mulOp expression           #mulExp
    | expression addOp expression           #addExp
    | expression compareOp expression       #compareExp
    | expression booleanOp expression       #booleanExp
    ;
    
mulOp: '*' | '/' | '%';
addOp: '+' | '-';
compareOp: '==' | '!=' | '>' | '<' | '>=' | '<=';
booleanOp: BOOLEAN_OPERATOR;

BOOLEAN_OPERATOR: 'and' | 'or' | 'not';
    
constant: INTEGER | FLOAT | STRING | BOOLEAN | NULL;

INTEGER: [0-9]+;
FLOAT: [0-9]+ '.' [0-9]+;
STRING: ('"' ~'"'* '"') | ('\'' ~'\''* '\'');
BOOLEAN: 'true' | 'false';
NULL: 'null';  

block: '{' line* '}'; 

WS: [ \t\r\n]+ -> skip;
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;

 