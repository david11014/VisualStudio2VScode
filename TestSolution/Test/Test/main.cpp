#include <stdio.h>
#include <stdlib.h>
#include <windows.h>

#include "ChangeMachine.h"
#include "ChangeMachineDef.h"

INT main()
{
	CONST UINT nChangeValue[ CHANGE_TYPE_COUNT ] = { 1000, 500, 100, 50, 10, 5, 1 }; // Define each type change value

	while( TRUE ) {

		UINT nChangeCount[ CHANGE_TYPE_COUNT ] = { 0, 0, 0, 0, 0, 0, 0 };

		printf( "How much money do you want to exchange?\n" );
		UINT nMoney = GetUserMoney();

		// Ask user want to exchange which change, and exchange money
		ExchangeMoney( nMoney, nChangeCount, nChangeValue , CHANGE_TYPE_COUNT );

		// Preview result
		printf( "It will exchange: " );
		ShowExchangeResult( nChangeCount, nChangeValue, CHANGE_TYPE_COUNT );
		
		// Ask user want to exchange or not
		printf( "Do you want to exchange?(y/n)\n" );
		if( AskYesNo() == TRUE ) {
			// Show result
			printf( "%d exchange: ", nMoney );
			ShowExchangeResult( nChangeCount, nChangeValue, CHANGE_TYPE_COUNT );
		}
		else {
			printf( "Cancel exchange\n" );
		}

		// Ask user close the program or not
		printf( "Do you want to continue?(y/n)\n" );
		if( AskYesNo() == FALSE ) {
			break;
		}

	}

	return 0;
}
