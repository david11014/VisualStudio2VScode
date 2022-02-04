#include <stdio.h>
#include <stdlib.h>
#include <conio.h>
#include <windows.h>

#include "ChangeMachine.h"
#include "ChangeMachineDef.h"

// Get user input and return the amount of money. It will limit user's input charactor
UINT GetUserMoney( void )
{
	CONST UINT nInputSize = 10;
	CHAR szUserInput[ nInputSize ] = ""; // User input string szUserInput

	UINT nSringLen = 0;
	while( TRUE ) {

		CHAR nInpurChar = getch();

		// Check user's input charactor
		if( nInpurChar == KEY_RETURN && nSringLen > 0 ) {
			printf("\n");
			break;
		}
		else if( nInpurChar == KEY_BACKSPACE && nSringLen > 0 ) {
			nSringLen--;
			PRINT_BACKSPACE();
		}
		// Input '0' at first
		else if( nInpurChar == '0' && nSringLen == 0) {
			continue;
		}
		// Input '0'~'9'
		else if( nInpurChar >= '0' && nInpurChar <= '9' && nSringLen < nInputSize - 1) {
			szUserInput[ nSringLen ] = nInpurChar;
			nSringLen++;
			printf( "%c", nInpurChar );
		}
	}

	szUserInput[ nSringLen ] = '\0'; // Make char array to string

	return strtoul( szUserInput, NULL, 10 );
}

// Ask yes/no question, yes return true, no return false. If reply unexpected answer, it will ask again.
BOOL AskYesNo( void )
{
	BOOL bAns = TRUE;
	BOOL bHasAns = FALSE;
	while( TRUE ) {

		CHAR nInpurChar = getch();

		if( bHasAns == FALSE ) {

			// Judge the answer
			if( nInpurChar == 'y' || nInpurChar == 'Y' ) {
				printf( "%c", nInpurChar );
				bAns = TRUE;
				bHasAns = TRUE;
			}
			else if( nInpurChar == 'n' || nInpurChar == 'N' ) {
				printf( "%c", nInpurChar );
				bAns = FALSE;
				bHasAns = TRUE;
			}
		}
		else if( bHasAns == TRUE ) {

			if( nInpurChar == KEY_RETURN) {
				printf( "\n" );
				return bAns;
			}
			else if( nInpurChar == KEY_BACKSPACE )
			{
				PRINT_BACKSPACE();
				bHasAns = FALSE;
			}
		}
	}
}

// Ask user want to exchange which change, and exchange money
VOID ExchangeMoney( UINT nMoney, UINT *pChangeCount, CONST UINT *pChangeValue , CONST UINT nCount )
{
	INT nMoneyTemp = nMoney;

	for( INT i = 0; i < ( nCount - 1 ); i++ ) {

		// Check money can be exchange
		if( nMoneyTemp < pChangeValue[ i ] ) {
			pChangeCount[ i ] = 0;
			continue;
		}

		//Ask and exchange money
		printf( "Do you want to exchange %d(y/n)\n", pChangeValue[ i ] );
		if( AskYesNo() == TRUE ) {
			pChangeCount[ i ] = ( INT )( nMoneyTemp / pChangeValue[ i ] );
			nMoneyTemp = nMoneyTemp % pChangeValue[ i ];
		}
	}

	pChangeCount[ nCount - 1 ] = nMoneyTemp;
}

VOID ShowExchangeResult( CONST UINT *pChangeCount, CONST UINT *pChangeValue , UINT nCount )
{
	for( INT i = 0; i < nCount; i++ ) {
		if( pChangeCount[ i ] != 0 ) {
			printf( "%dX%d, ", pChangeValue[ i ], pChangeCount[ i ] );
		}
	}

	printf( "\n" );
}
