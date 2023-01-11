grammar IterkoczeScript;

program: line* EOF;
line: statement | ifBlock | forBlock | whileBlock | foreachBlock | functionDefinition | structOperation;

statement: (variableDefinition | catapult | useDirective | arrayOperation | assingment | listOperation 
        | structMemberDefinition | structOperation | expression
        | functionCall | returnStatement | dictionaryOperation) ';';

dictionaryOperation
    : 'new Dictionary' IDENTIFIER                       #dictionaryCreation
    | IDENTIFIER '[' STRING ']' '=' expression          #dictionaryAssingment
    ;

listOperation
    : 'new List' IDENTIFIER                             #listCreation
    ;

structOperation
    : 'new Struct' IDENTIFIER IDENTIFIER                #structCreation
    | IDENTIFIER ':' IDENTIFIER '=' expression          #structAssingment
    | DEFINE 'Struct' IDENTIFIER '->' block                  #structDefinition
    ;
    
arrayOperation
    : 'new Array' IDENTIFIER '[' expression ']'                 #arrayCreation
    | IDENTIFIER '[' expression ']' ':' IDENTIFIER              #arrayStructMemberAccess
    | IDENTIFIER '[' INTEGER ']' '=' expression                 #arrayAssingment
    | IDENTIFIER '[' INTEGER ']' ':' IDENTIFIER '=' expression  #arrayStructMemberAccessAssingment
    ;

returnStatement: 'give' expression;

ifBlock: 'perhaps' block  IF '(' expression ')' ';' ('otherwise' elseIfBlock)?;

forBlock: 'for' '(' assingment ';' expression ';' INTEGER ')' block;

foreachBlock: 'each' '(' IDENTIFIER 'in' expression ')' block;

elseIfBlock: block | ifBlock;

whileBlock: WHILE '(' expression ')' block ('else' elseIfBlock)?; //RECENT UPDATE WITH `?` WATCH OUT

WHILE: 'while';

IF: 'if';

functionDefinition: 'meth' IDENTIFIER '->' block;

functionCall: IDENTIFIER '(' (expression (',' expression)*)? ')';

methodCall: IDENTIFIER '.' functionCall;

DEFINE: 'def' | 'define';

structMemberDefinition: 'member' IDENTIFIER;

useDirective: ('@use' || '#use') IDENTIFIER;

catapult: 'catapult' IDENTIFIER;

variableDefinition: 'remember' GLOBAL? CONST? IDENTIFIER '=' expression;

expression
    : constant                                              #constantExp
    | '$' INTEGER                                           #argumentIdentifierExp
    | '$' INTEGER '=' expression                            #argumentAssingmentExp
    | returnStatement                                       #returnStatementExp
    | IDENTIFIER ':' IDENTIFIER                             #structMemberAccessExp
    | methodCall                                            #methodCallExp
    | IDENTIFIER '[' expression ']'                         #arrayAccessExp
    | IDENTIFIER                                            #identifierExp
    | functionCall                                          #functionCallExp
    | functionDefinition                                    #functionDefinitionExp
    | '(' expression ')'                                    #parenthesizedExp
    | INVERT_OPERATOR expression                            #notExp
    | expression mathOp expression                          #mathExp
    | expression compareOp expression                       #compareExp
    | expression booleanOp expression                       #booleanExp
    | listOperation                                         #listOperationExp
    | arrayOperation                                        #arrayOperationExp
    | 'crack'                                               #crackLoopExp
    ;
    
assingment: IDENTIFIER '=' expression;  
    
mathOp: '+' | '-' | '*' | '/' | '%' | '^';
compareOp: '==' | '!=' | '>' | '<' | '>=' | '<=';
booleanOp: BOOLEAN_OPERATOR;

BOOLEAN_OPERATOR: 'and' | 'or';
INVERT_OPERATOR: 'not' | '!';
    
//VAR_MOD: ('global' & 'const') ;
GLOBAL: 'global';
CONST: 'const';

constant: INTEGER | FLOAT | STRING | BOOLEAN | NULL;

INTEGER: [0-9]+;
FLOAT: [0-9]+ '.' [0-9]+;
STRING: ('"' ~'"'* '"') | ('\'' ~'\''* '\'');
BOOLEAN: 'true' | ('false' | 'flase' | 'non' | 'lie');
NULL: 'null';  

block: '{' line* '}'; 

WS: [ \t\r\n]+ -> skip;
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;
LINE_COMMENT
    : '//' ~[\r\n]* -> skip
;