''---------------------------------------------------------------------------
''   Arquivo de chave RSA 1024
''   Gravado em [C:\TQSAPP\LINK\APLIC\LINK\TQSProt\LINK_EXE_CPU.H] em 18-11-2025 20:38:43
''   Pelo programa TQSASSIN.EXE
''   Chave p\xfablica correspondente ao arquivo [C:\TQSAPP\LINK\APLIC\LINK\TQSProt\LINK.EXE]
''---------------------------------------------------------------------------
Public Const glb_nchapubl As Integer = 160
Public Sub glb_chapubl(chapubl() As Byte)
    Dim theArray() As Variant
    theArray() = Array( _
	&H30, &H81, &H9D, &H30, &H0D, &H06, &H09, &H2A, &H86, &H48,  _
	&H86, &HF7, &H0D, &H01, &H01, &H01, &H05, &H00, &H03, &H81,  _
	&H8B, &H00, &H30, &H81, &H87, &H02, &H81, &H81, &H00, &H9D,  _
	&H8E, &HF8, &H7A, &HC9, &H04, &H02, &HF8, &H26, &H48, &HDB,  _
	&HA0, &HED, &H1A, &H59, &H48, &H19, &H89, &HB6, &H24, &H26,  _
	&H26, &H6E, &HFF, &H90, &HDD, &HF5, &H63, &HC2, &H9C, &H76,  _
	&HFA, &HC1, &H46, &HA6, &H32, &HBD, &HE3, &H6F, &HB9, &HF0,  _
	&H7E, &H28, &HE2, &H79, &H92, &H24, &HA3, &H0D, &H4B, &H5B,  _
	&HD0, &H6A, &H5F, &HCC, &H1C, &HA0, &H77, &H89, &H25, &H13,  _
	&H3B, &H69, &HC6, &H82, &H42, &H1E, &H32, &H67, &HA8, &HB2,  _
	&H8D, &HBC, &H1A, &HEC, &HD9, &HFD, &HD4, &HE6, &HAF, &H9C,  _
	&H94, &HA2, &HB0, &HF1, &H45, &H55, &HB6, &HA5, &H22, &H9B,  _
	&H6F, &H0B, &HC3, &H97, &H5B, &H67, &H84, &H48, &HBE, &H82,  _
	&HCB, &HF6, &H5B, &HA9, &HFC, &H4D, &HF8, &HCA, &H3A, &HC3,  _
	&HC2, &H52, &HFB, &HFE, &H66, &H98, &H70, &H6E, &H57, &H72,  _
	&HB2, &H24, &H6A, &HEB, &H54, &HBC, &H9D, &H02, &H01, &H11   _
    )
    ReDim chapubl(0 To glb_nchapubl - 1)
    Dim i As Integer
    For i = 0 To glb_nchapubl - 1
        chapubl(i) = theArray(i)
    Next
End Sub
