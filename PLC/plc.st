PROGRAM program0
  VAR
    PI_Taster_Links AT %IX0.0 : BOOL;
    PI_LED_rot AT %QX0.0 : BOOL;
    PI_LED_gruen AT %QX0.1 : BOOL;
    PI_Taster_Rechts AT %IX0.1 : BOOL;
    PI_LED_blau AT %QX0.2 : BOOL;
    E_STOP AT %QX4.0 : BOOL;
    E_Start AT %QX4.1 : BOOL;
    E_Reset AT %QX4.2 : BOOL;
    E_Hand_Automatik AT %QX4.3 : BOOL;
    E_Links AT %QX4.4 : BOOL;
    E_Rechts AT %QX4.5 : BOOL;
    E_Drehmelder AT %QX4.6 : BOOL;
    A_H1 AT %QX5.0 : BOOL;
    A_H2 AT %QX5.1 : BOOL;
    A_H3 AT %QX5.2 : BOOL;
    A_H4 AT %QX5.3 : BOOL;
    A_Motor_links AT %QX5.4 : BOOL;
    A_Motor_rechts AT %QX5.5 : BOOL;
    A_Motor_MaxSpeed AT %QX5.6 : BOOL;
  END_VAR
  VAR
    NOT25_OUT : BOOL;
    NOT26_OUT : BOOL;
  END_VAR

  PI_LED_rot := E_Links;
  PI_LED_gruen := E_Rechts;
  NOT25_OUT := NOT(PI_Taster_Links);
  A_Motor_links := NOT25_OUT;
  NOT26_OUT := NOT(PI_Taster_Rechts);
  A_Motor_rechts := NOT26_OUT;
END_PROGRAM


CONFIGURATION Config0

  RESOURCE Res0 ON PLC
    TASK task0(INTERVAL := T#20ms,PRIORITY := 0);
    PROGRAM instance0 WITH task0 : program0;
  END_RESOURCE
END_CONFIGURATION
