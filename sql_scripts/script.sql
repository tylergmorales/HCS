USE [master]
GO
/****** Object:  Database [isp]    Script Date: 4/5/2017 9:56:26 PM ******/
CREATE DATABASE [isp]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'isp', FILENAME = N'C:\Users\Mike\isp.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'isp_log', FILENAME = N'C:\Users\Mike\isp_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [isp] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [isp].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [isp] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [isp] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [isp] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [isp] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [isp] SET ARITHABORT OFF 
GO
ALTER DATABASE [isp] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [isp] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [isp] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [isp] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [isp] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [isp] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [isp] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [isp] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [isp] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [isp] SET  DISABLE_BROKER 
GO
ALTER DATABASE [isp] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [isp] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [isp] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [isp] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [isp] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [isp] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [isp] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [isp] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [isp] SET  MULTI_USER 
GO
ALTER DATABASE [isp] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [isp] SET DB_CHAINING OFF 
GO
ALTER DATABASE [isp] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [isp] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [isp] SET DELAYED_DURABILITY = DISABLED 
GO
USE [isp]
GO
/****** Object:  Table [dbo].[hospital]    Script Date: 4/5/2017 9:56:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[hospital](
	[hospitalId] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[hospitalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[patient]    Script Date: 4/5/2017 9:56:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[patient](
	[patientId] [int] IDENTITY(1,1) NOT NULL,
	[gender] [varchar](50) NULL,
	[streetAddress] [varchar](50) NULL,
	[city] [varchar](50) NULL,
	[state] [varchar](2) NULL,
	[zip] [int] NULL,
	[motherId] [int] NULL,
	[fatherId] [int] NULL,
	[birthDate] [datetime] NULL,
	[educationEarned] [varchar](50) NULL,
	[hispanic] [varchar](50) NULL,
	[race] [varchar](50) NULL,
	[height] [int] NULL,
	[weight] [int] NULL,
	[married] [bit] NULL,
	[birthplace] [varchar](50) NULL,
	[phone] [varchar](10) NULL,
	[firstName] [varchar](50) NOT NULL,
	[middleName] [varchar](50) NULL,
	[lastName] [varchar](50) NOT NULL,
	[suffix] [varchar](50) NULL,
	[maidenName] [varchar](50) NULL,
	[birthFacility] [varchar](50) NULL,
	[birthLocation] [varchar](50) NULL,
	[birthCounty] [varchar](50) NULL,
	[previousFirstName] [varchar](50) NULL,
	[previousMiddleName] [varchar](50) NULL,
	[previousLastName] [varchar](50) NULL,
	[previousSuffix] [varchar](50) NULL,
	[aptNo] [varchar](50) NULL,
	[insideCity] [bit] NULL,
	[mailingStreetAddress] [varchar](50) NULL,
	[ssn] [varchar](9) NULL,
PRIMARY KEY CLUSTERED 
(
	[patientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[record]    Script Date: 4/5/2017 9:56:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[record](
	[recordId] [int] IDENTITY(1,1) NOT NULL,
	[patientId] [int] NOT NULL,
	[certiferName] [varchar](50) NULL,
	[certifierTitle] [varchar](50) NULL,
	[certifierDate] [date] NULL,
	[filedDate] [date] NULL,
	[paternityAck] [bit] NULL,
	[ssnRequested] [bit] NULL,
	[facilityId] [varchar](50) NULL,
	[birthFacility] [varchar](50) NULL,
	[homebirth] [bit] NULL,
	[attendantName] [varchar](50) NULL,
	[attendantNpi] [varchar](50) NULL,
	[attendantTitle] [varchar](50) NULL,
	[motherTransferred] [bit] NULL,
	[transferFacility] [varchar](50) NULL,
	[firstPrenatal] [date] NULL,
	[lastPrenatal] [date] NULL,
	[totalPrenatal] [varchar](3) NULL,
	[motherPreWeight] [int] NULL,
	[motherPostWeight] [int] NULL,
	[motherDeliveryWeight] [int] NULL,
	[hadWic] [bit] NULL,
	[previousBirthLiving] [int] NULL,
	[previousBirthDead] [int] NULL,
	[lastLiveBirth] [date] NULL,
	[otherBirthOutcomes] [int] NULL,
	[lastOtherOutcome] [date] NULL,
	[cigThreeBefore] [varchar](3) NULL,
	[packThreeBefore] [varchar](3) NULL,
	[cigFirstThree] [varchar](3) NULL,
	[packFirstThree] [varchar](3) NULL,
	[cigSecondThree] [varchar](3) NULL,
	[packSecondThree] [varchar](3) NULL,
	[cigThirdTri] [varchar](3) NULL,
	[packThirdTri] [varchar](3) NULL,
	[paymentSource] [varchar](50) NULL,
	[dateLastMenses] [date] NULL,
	[diabetesPrepregancy] [bit] NULL,
	[diabetesGestational] [bit] NULL,
	[hyperTensionPrepregnancy] [bit] NULL,
	[hyperTensionGestational] [bit] NULL,
	[hyperTensionEclampsia] [bit] NULL,
	[prePreTerm] [bit] NULL,
	[prePoorOutcome] [bit] NULL,
	[resultInfertility] [bit] NULL,
	[fertilityDrug] [bit] NULL,
	[assistedTech] [bit] NULL,
	[previousCesarean] [bit] NULL,
	[previousCesareanAmount] [int] NULL,
	[gonorrhea] [bit] NULL,
	[syphilis] [bit] NULL,
	[chlamydia] [bit] NULL,
	[hepB] [bit] NULL,
	[hepC] [bit] NULL,
	[cervicalCerclage] [bit] NULL,
	[tocolysis] [bit] NULL,
	[externalCephalic] [bit] NULL,
	[preRuptureMembrane] [bit] NULL,
	[preLabor] [bit] NULL,
	[proLabor] [bit] NULL,
	[inductionLabor] [bit] NULL,
	[augmentationLabor] [bit] NULL,
	[nonvertex] [bit] NULL,
	[steroids] [bit] NULL,
	[antibotics] [bit] NULL,
	[chorioamnionitis] [bit] NULL,
	[meconium] [bit] NULL,
	[fetalIntolerance] [bit] NULL,
	[epidural] [bit] NULL,
	[unsuccessfulForceps] [bit] NULL,
	[unsuccessfulVacuum] [bit] NULL,
	[cephalic] [bit] NULL,
	[breech] [bit] NULL,
	[otherFetalPresentation] [bit] NULL,
	[finalSpontaneous] [bit] NULL,
	[finalForcepts] [bit] NULL,
	[finalVacuum] [bit] NULL,
	[finalCesarean] [bit] NULL,
	[finalTrialOfLabor] [bit] NULL,
	[maternalTransfusion] [bit] NULL,
	[perinealLaceration] [bit] NULL,
	[rupturedUterus] [bit] NULL,
	[hysterectomy] [bit] NULL,
	[admitICU] [bit] NULL,
	[unplannedOperating] [bit] NULL,
	[fiveMinAgpar] [varchar](50) NULL,
	[tenMinAgpar] [varchar](50) NULL,
	[plurality] [varchar](50) NULL,
	[birthOrder] [varchar](50) NULL,
	[ventImmedite] [bit] NULL,
	[ventSixHours] [bit] NULL,
	[nicu] [bit] NULL,
	[surfactant] [bit] NULL,
	[neoNatalAntibiotics] [bit] NULL,
	[seizureDysfunction] [bit] NULL,
	[birthInjury] [bit] NULL,
	[anencephaly] [bit] NULL,
	[meningomyelocele] [bit] NULL,
	[cyanotic] [bit] NULL,
	[congenital] [bit] NULL,
	[omphalocele] [bit] NULL,
	[gastroschisis] [bit] NULL,
	[limbReduction] [bit] NULL,
	[cleftLip] [bit] NULL,
	[cleftPalate] [bit] NULL,
	[downConfirmed] [bit] NULL,
	[downPending] [bit] NULL,
	[suspectedConfirmed] [bit] NULL,
	[suspectedPending] [bit] NULL,
	[Hypospadias] [bit] NULL,
	[infantTransferred] [bit] NULL,
	[infantTransferFacility] [varchar](50) NULL,
	[infantLiving] [varchar](50) NULL,
	[breastFed] [bit] NULL,
	[hospitalId] [int] NULL,
	[childId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[recordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[role]    Script Date: 4/5/2017 9:56:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[role](
	[roleId] [int] IDENTITY(1,1) NOT NULL,
	[title] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[roleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[staff]    Script Date: 4/5/2017 9:56:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[staff](
	[staffId] [int] IDENTITY(1,1) NOT NULL,
	[hospitalId] [int] NOT NULL,
	[roleId] [int] NOT NULL,
	[firstName] [varchar](50) NOT NULL,
	[lastName] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[staffId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[record]  WITH CHECK ADD FOREIGN KEY([childId])
REFERENCES [dbo].[patient] ([patientId])
GO
ALTER TABLE [dbo].[record]  WITH CHECK ADD FOREIGN KEY([hospitalId])
REFERENCES [dbo].[hospital] ([hospitalId])
GO
ALTER TABLE [dbo].[record]  WITH CHECK ADD FOREIGN KEY([patientId])
REFERENCES [dbo].[patient] ([patientId])
GO
ALTER TABLE [dbo].[staff]  WITH CHECK ADD FOREIGN KEY([hospitalId])
REFERENCES [dbo].[hospital] ([hospitalId])
GO
ALTER TABLE [dbo].[staff]  WITH CHECK ADD FOREIGN KEY([roleId])
REFERENCES [dbo].[role] ([roleId])
GO
USE [master]
GO
ALTER DATABASE [isp] SET  READ_WRITE 
GO
