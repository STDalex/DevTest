��� ������������ ������������� ����������
1. ��������� ������������ ��������� APS (� TCP �������� ����) � ����������� "testing" �, ���� ��� ���������� ������������� ����� �������������� ������ ���������, "nohttp".
2. ������������ ����� � �������

�������� ���������� test.html

<?xml version="1.0" encoding="windows-1251"?>
<test>
  <devices><Name>��-�27</Name></devices>	������ ���������, ��� ������� �������� ���� ����
  <devices><Name>���-�2</Name></devices>
  <devices><Name>���-�2</Name></devices>
  <devices><Name>��-01</Name></devices>
  <devices><Name>��-01</Name></devices>
  <devices><Name>��-01 S2</Name></devices>
  <devices><Name>��-01 E S2</Name></devices>
  <devices><Name>����</Name></devices>
  <devices><Name>���� �</Name></devices>
  <devices><Name>���� � S2</Name></devices>
  <devices><Name>���� E TCC</Name></devices>
  <devices><Name>���� S2</Name></devices>
  <devices><Name>���� TCC</Name></devices>
  <devices><Name>���-�</Name></devices>
  <devices><Name>���-� �</Name></devices>
  <devices><Name>���-� � S2</Name></devices>
  <devices><Name>���-� E TCC</Name></devices>
  <devices><Name>���-� S2</Name></devices>
  <devices><Name>���-� TCC</Name></devices>
  <Options>
    <���������������������>test_tcp_1100_2000_device_settings.xml</���������������������> ���� � ���������� ������������ �� ������ ������, ������ ������ � ����� � ������, ����������� HTTP �������� ����� ��������� ���������� �� ������
    <�������������>
        <Key>������� �������</Key>
        <Value>1100</Value>
    </�������������>
    <�������������>
        <Key>������� �������������</Key>
        <Value>2000000</Value>
    </�������������>
    <�������������>
        <Key>������ �����</Key>				�� � ����� ������ 2 �����������
        <Value>7</Value>
    </�������������>
    <�������������>
        <Key>��� �����</Key>
        <Value>C:\Users\FM4_TPC_3_4_2964_16.bit</Value>		���������� ���� � ����� � ��������, ������ ������ �� ��� � ����
    </�������������>
    <�������������>
        <Key>����������</Key>
        <Value>10</Value>
    </�������������>
    <�������������>
        <Key>��������</Key>
        <Value>6</Value>
    </�������������>
    <�������������>
        <Key>�������� �������</Key>
        <Value>false</Value>
    </�������������>
    <�������������>
        <Key>�����</Key>
        <Value>true</Value>
    </�������������>
    <�������������>
        <Key>�����������</Key>
        <Value>true</Value>
    </�������������>
    <�������������>
        <Key>����������� ���������������</Key>
        <Value>true</Value>
    </�������������>

  </Options>
  <expected_values>	�������� �������� ����������� ����������
    <�������������_������������_��������_���>Green</�������������_������������_��������_���>
    <���_��������_����_�_min>1911,0</���_��������_����_�_min>
    <���_��������_����_�_max>1911,4</���_��������_����_�_max>
    <Eb_N0_min>11,0</Eb_N0_min>
    <Eb_N0_max>13,0</Eb_N0_max>
    <���_min>-17</���_min>
    <���_max>-7</���_max>
    <����_min>-8</����_min>
    <����_max>-5</����_max>
  </expected_values>
</test>

������ �����		Value
8  ��� �-����� ����       0
8  ��� �-����� ������	  1
8  ��� �������� ����	  2
8  ��� �������� ������	  3
8  ��� ����������� ����	  4
8  ��� ����������� ������ 5
16 ��� �������� ����      6
16 ��� �������� ������ 	  7
16 ��� ����������� ����   8
16 ��� ����������� ������ 9 

3. ������������ ����� config.xml ���� (������ ������ � ���������� � ���������� ������������)
<aps_socket> ����� HTTP ������� ��������� ���������� �������������� </aps_socket>
<uvgs_ip> IP ������ TCP ������� ���������� ���� </uvgs_ip>
<uvgs_port> ���� TCP ������� ���������� ����, �� ������ 28300 </uvgs_port>
<test_files_folder> ����� � ������� ������ </test_files_folder>
<test_file><file_name> ��� ����� � ������, ������� � <test_files_folder>, ����� ���� ���������, ����������� �� ������� </file_name></test_file>

�������� ���������� config.xml

<?xml version="1.0" encoding="windows-1251"?>
<config>
  <aps_socket>192.168.33.26:8000</aps_socket>
  <uvgs_ip>192.168.33.26</uvgs_ip>
  <uvgs_port>28300</uvgs_port>
  <test_files_folder>test_files</test_files_folder>
  <test_file><file_name>test_tpc_1100_2000.xml</file_name></test_file>
  <test_file><file_name>test_tpc_2100_1000.xml</file_name></test_file>
  <test_file><file_name>test_dvb_s2_1200.xml</file_name></test_file>
</config>

4. ��������� DeviceTest.exe � ��������� ��������� ������. ���������� ������������ � ������� � ��������������� html ����� ( reports/Report_�����.html )
