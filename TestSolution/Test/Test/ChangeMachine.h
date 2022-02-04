#ifndef CHANGE_MACHINE_H
#define CHANGE_MACHINE_H

// Get user input and return the amount of money. It will limit user's input charactor
UINT GetUserMoney( void );

// Ask yes/no question, yes return true, no return false. If reply unexpected answer, it will ask again.
BOOL AskYesNo( void );

// Ask user want to exchange which change, and exchange money
VOID ExchangeMoney( UINT nMoney, UINT *pChangeCount, CONST UINT *pChangeValue , CONST UINT nCount );

VOID ShowExchangeResult( CONST UINT *pChangeCount, CONST UINT *pChangeValue , CONST UINT nCount );

#endif
