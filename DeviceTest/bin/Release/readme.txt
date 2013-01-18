Для тестирования демодуляторов необходимо
1. Запустить доработанную программу APS (с TCP сервером УВГС) с параметрами "testing" и, если для управления демодуляторми будет использоваться другая программа, "nohttp".
2. Сформировать файлы с тестами

типичное содержимое test.html

<?xml version="1.0" encoding="windows-1251"?>
<test>
  <devices><Name>Нс-Д27</Name></devices>	список устройств, для которых доступен этот тест
  <devices><Name>ПДУ-В2</Name></devices>
  <devices><Name>ЦПУ-А2</Name></devices>
  <devices><Name>СД-01</Name></devices>
  <devices><Name>РД-01</Name></devices>
  <devices><Name>РД-01 S2</Name></devices>
  <devices><Name>РД-01 E S2</Name></devices>
  <devices><Name>МПДО</Name></devices>
  <devices><Name>МПДО Е</Name></devices>
  <devices><Name>МПДО Е S2</Name></devices>
  <devices><Name>МПДО E TCC</Name></devices>
  <devices><Name>МПДО S2</Name></devices>
  <devices><Name>МПДО TCC</Name></devices>
  <devices><Name>УПД-М</Name></devices>
  <devices><Name>УПД-М Е</Name></devices>
  <devices><Name>УПД-М Е S2</Name></devices>
  <devices><Name>УПД-М E TCC</Name></devices>
  <devices><Name>УПД-М S2</Name></devices>
  <devices><Name>УПД-М TCC</Name></devices>
  <Options>
    <НастройкаДемодулятора>test_tcp_1100_2000_device_settings.xml</НастройкаДемодулятора> файл с настройкой демодулятора на данный сигнал, должен лежать в папке с файлом, генериуется HTTP сервером после настройки устройства на сигнал
    <НастройкаУВГС>
        <Key>Несущая частота</Key>
        <Value>1100</Value>
    </НастройкаУВГС>
    <НастройкаУВГС>
        <Key>Частота дискретизации</Key>
        <Value>2000000</Value>
    </НастройкаУВГС>
    <НастройкаУВГС>
        <Key>Формат файла</Key>				см в конце пункта 2 расшифровку
        <Value>7</Value>
    </НастройкаУВГС>
    <НастройкаУВГС>
        <Key>Имя файла</Key>
        <Value>C:\Users\FM4_TPC_3_4_2964_16.bit</Value>		абсолютный путь к файлу с сигналом, должен лежать на ЭВМ с УВГС
    </НастройкаУВГС>
    <НастройкаУВГС>
        <Key>Аттенюатор</Key>
        <Value>10</Value>
    </НастройкаУВГС>
    <НастройкаУВГС>
        <Key>Усиление</Key>
        <Value>6</Value>
    </НастройкаУВГС>
    <НастройкаУВГС>
        <Key>Инверсия спектра</Key>
        <Value>false</Value>
    </НастройкаУВГС>
    <НастройкаУВГС>
        <Key>Выход</Key>
        <Value>true</Value>
    </НастройкаУВГС>
    <НастройкаУВГС>
        <Key>Кэширование</Key>
        <Value>true</Value>
    </НастройкаУВГС>
    <НастройкаУВГС>
        <Key>Циклическое воспроизведение</Key>
        <Value>true</Value>
    </НастройкаУВГС>

  </Options>
  <expected_values>	ожидаемы значения считываемых парамтеров
    <Синхронизация_демодулятора_декодера_УКС>Green</Синхронизация_демодулятора_декодера_УКС>
    <Инф_скорость_кбит_с_min>1911,0</Инф_скорость_кбит_с_min>
    <Инф_скорость_кбит_с_max>1911,4</Инф_скорость_кбит_с_max>
    <Eb_N0_min>11,0</Eb_N0_min>
    <Eb_N0_max>13,0</Eb_N0_max>
    <АРУ_min>-17</АРУ_min>
    <АРУ_max>-7</АРУ_max>
    <ЦАРУ_min>-8</ЦАРУ_min>
    <ЦАРУ_max>-5</ЦАРУ_max>
  </expected_values>
</test>

Формат файла		Value
8  бит А-закон моно       0
8  бит А-закон стерео	  1
8  бит знаковый моно	  2
8  бит знаковый стерео	  3
8  бит беззнаковый моно	  4
8  бит беззнаковый стерео 5
16 бит знаковый моно      6
16 бит знаковый стерео 	  7
16 бит беззнаковый моно   8
16 бит беззнаковый стерео 9 

3. Сформировать верно config.xml файл (должен лежать в директории с программой тестирования)
<aps_socket> сокет HTTP сервера программы управления демодуляторами </aps_socket>
<uvgs_ip> IP адресс TCP сервера управления УВГС </uvgs_ip>
<uvgs_port> порт TCP сервера управления УВГС, по всегда 28300 </uvgs_port>
<test_files_folder> папка с файлами тестов </test_files_folder>
<test_file><file_name> имя файла с тестом, лежащим в <test_files_folder>, может быть несколько, запускаются по очереди </file_name></test_file>

типичное содержимое config.xml

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

4. Запустить DeviceTest.exe и дождаться окончания тестов. Результаты отображаются в консоли и сгенерированном html файле ( reports/Report_время.html )
