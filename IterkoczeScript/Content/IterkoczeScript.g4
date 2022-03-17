grammar IterkoczeScript;

program: line* EOF;
line: statement | ifBlock | forBlock | whileBlock | foreachBlock | functionDefinition | structOperation;

statement: (assingment | listOperation | arrayCreation | structMemberDefinition | structOperation | functionCall | returnStatement) ';';

listOperation
    : 'new List' IDENTIFIER                             #listCreation
    | IDENTIFIER '.Add' '(' expression ')'              #listAddOperation
    ;

structOperation
    : 'new' IDENTIFIER IDENTIFIER?              #structCreation
    | IDENTIFIER '.' IDENTIFIER '=' expression  #structAssingment
    | DEFINE 'struct' IDENTIFIER block          #structDefinition
    ;

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

structMemberDefinition: IDENTIFIER;

assingment: IDENTIFIER '=' expression | (IDENTIFIER '[' INTEGER ']' '=' expression);

arrayCreation: 'new array' IDENTIFIER '[' expression ']';

expression
    : constant                                              #constantExp
    | '$' INTEGER                                           #argumentIdentifierExp
    | returnStatement                                       #returnStatementExp
    | IDENTIFIER '.' IDENTIFIER                             #structMemberAccessExp
    | IDENTIFIER '[' expression ']' '.' IDENTIFIER          #arrayStructMemberAccessExp
    | IDENTIFIER '[' expression ']'                         #arrayAccessExp
    | IDENTIFIER                                            #identifierExp
    | functionCall                                          #functionCallExp
    | functionDefinition                                    #functionDefinitionExp
    | '(' expression ')'                                    #parenthesizedExp
    | INVERT_OPERATOR expression                            #notExp
    | expression mulOp expression                           #mulExp
    | expression addOp expression                           #addExp
    | expression compareOp expression                       #compareExp
    | expression booleanOp expression                       #booleanExp
    ;
    
mulOp: '*' | '/' | '%';
addOp: '+' | '-';
compareOp: '==' | '!=' | '>' | '<' | '>=' | '<=';
booleanOp: BOOLEAN_OPERATOR;

BOOLEAN_OPERATOR: 'and' | 'or';
INVERT_OPERATOR: 'not' | '!';
    
constant: INTEGER | FLOAT | STRING | BOOLEAN | NULL;

INTEGER: [0-9]+;
FLOAT: [0-9]+ '.' [0-9]+;
STRING: ('"' ~'"'* '"') | ('\'' ~'\''* '\'');
BOOLEAN: 'true' | 'false';
NULL: 'null';  

block: '{' line* '}'; 

WS: [ \t\r\n]+ -> skip;
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;

 