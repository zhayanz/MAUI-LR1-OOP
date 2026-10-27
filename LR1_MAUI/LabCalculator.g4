grammar LabCalculator;

/*
 * Parser Rules
 */

compileUnit : expression EOF;

expression :
    LPAREN expression RPAREN #ParenthesizedExpr
    |expression EXPONENT expression #ExponentialExpr
    | expression operatorToken=(MULTIPLY | DIVIDE) expression #MultiplicativeExpr
    | expression operatorToken=(ADD | SUBTRACT) expression #AdditiveExpr
    | MAX LPAREN expression ',' expression RPAREN #MaxExpr
    | MIN LPAREN expression ',' expression RPAREN #MinExpr
    | INC LPAREN expression RPAREN #IncExpr
    | DEC LPAREN expression RPAREN #DecExpr
    | NUMBER #NumberExpr
    | IDENTIFIER #IdentifierExpr
    ;

/*
 * Lexer Rules
 */

NUMBER : INT ('.' INT)?;
IDENTIFIER : [a-zA-Z]+[1-9][0-9]*;

INT : ('0'..'9')+;

MAX : 'max';
MIN : 'min';
INC : 'inc';
DEC : 'dec';

EXPONENT : '^';
MULTIPLY : '*';
DIVIDE : '/';
SUBTRACT : '-';
ADD : '+';
LPAREN : '(';
RPAREN : ')';

WS : [ \t\r\n] -> channel(HIDDEN);
