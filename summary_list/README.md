# Summary List 프로젝트 요약

## 프로젝트 개요

WPF를 사용하여 아이템 목록을 표시하고, 페이지별로 나누어 보여주며, BMP 파일로 저장할 수 있는 애플리케이션입니다.

## 주요 기능

1. 아이템 목록 표시
   - 가변적인 레이아웃 (화면 크기에 따라 자동 조정)
   - 페이지별 표시
   - 체크 상태 표시 (V/X 기호)
2. 페이지 네비게이션
   - 이전/다음 페이지 이동
   - 현재 페이지 정보 표시
3. BMP 저장 기능
   - 현재 페이지 저장
   - 모든 페이지 저장

## 구현된 컴포넌트

### 1. ViewModels

- **MainViewModel.cs**
  - 아이템 목록 관리
  - 페이지 네비게이션 로직
  - BMP 저장 기능 구현
- **BaseViewModel.cs**
  - INotifyPropertyChanged 구현
  - 속성 변경 알림 기능
- **RelayCommand.cs**
  - ICommand 구현
  - 커맨드 패턴 지원

### 2. Models

- **SummaryItem.cs**
  - 아이템 데이터 모델
  - 텍스트, 체크 상태, 체크 기호 속성

### 3. Converters

- **BoolToColorConverter.cs**
  - 아이템 배경색 변환
  - 다크 테마에 맞춘 통일된 배경색 적용
- **CheckToColorConverter.cs**
  - 체크 기호 색상 변환
  - 체크 상태에 따라 Green/Red 색상 적용

### 4. Views

- **MainView.xaml**
  - 다크 테마 UI 구현
  - 아이템 목록 표시
  - 페이지 네비게이션 컨트롤
  - 저장 버튼

## UI/UX 특징

1. 다크 테마 적용

   - 배경색: RGB(30, 30, 30)
   - 아이템 배경색: RGB(45, 45, 45)
   - 테두리 색상: RGB(62, 62, 62)
   - 텍스트 색상: 흰색

2. 아이템 디자인

   - 통일된 배경색 사용
   - 체크 상태에 따른 기호 색상 구분 (Green/Red)
   - 깔끔한 레이아웃과 여백

3. 반응형 디자인
   - 화면 크기에 따른 자동 레이아웃 조정
   - 가변적인 아이템 배치

## BMP 저장 기능

1. 현재 페이지 저장

   - 다크 테마 적용
   - 아이템 레이아웃 유지
   - 체크 상태 표시

2. 모든 페이지 저장
   - 각 페이지별로 개별 파일 생성
   - 페이지 번호 포함된 파일명 사용

## 기술 스택

- WPF (Windows Presentation Foundation)
- C#
- XAML
- MVVM 패턴
- IValueConverter 인터페이스
- DrawingVisual을 이용한 BMP 생성
