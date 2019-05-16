USE [master]
GO
/****** Object:  Database [NoorCare]    Script Date: 5/16/2019 9:17:04 PM ******/
CREATE DATABASE [NoorCare]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'NoorCare', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\NoorCare.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'NoorCare_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\NoorCare_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [NoorCare] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [NoorCare].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [NoorCare] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [NoorCare] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [NoorCare] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [NoorCare] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [NoorCare] SET ARITHABORT OFF 
GO
ALTER DATABASE [NoorCare] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [NoorCare] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [NoorCare] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [NoorCare] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [NoorCare] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [NoorCare] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [NoorCare] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [NoorCare] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [NoorCare] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [NoorCare] SET  DISABLE_BROKER 
GO
ALTER DATABASE [NoorCare] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [NoorCare] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [NoorCare] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [NoorCare] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [NoorCare] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [NoorCare] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [NoorCare] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [NoorCare] SET RECOVERY FULL 
GO
ALTER DATABASE [NoorCare] SET  MULTI_USER 
GO
ALTER DATABASE [NoorCare] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [NoorCare] SET DB_CHAINING OFF 
GO
ALTER DATABASE [NoorCare] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [NoorCare] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [NoorCare] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'NoorCare', N'ON'
GO
ALTER DATABASE [NoorCare] SET QUERY_STORE = OFF
GO
USE [NoorCare]
GO
/****** Object:  Table [dbo].[Client]    Script Date: 5/16/2019 9:17:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Client](
	[id] [int] NULL,
	[clientId] [varchar](50) NOT NULL,
	[firstName] [varchar](50) NULL,
	[middleName] [varchar](50) NULL,
	[lastName] [varchar](50) NULL,
	[gender] [int] NULL,
	[Address] [varchar](max) NULL,
	[city] [varchar](50) NULL,
	[state] [varchar](50) NULL,
	[country] [varchar](50) NULL,
	[mobileNo] [int] NULL,
	[emailId] [varchar](50) NULL,
	[createDate] [datetime] NULL,
	[modifyDate] [datetime] NULL,
	[modifyBy] [varchar](50) NULL,
 CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED 
(
	[clientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClientDoc]    Script Date: 5/16/2019 9:17:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClientDoc](
	[id] [int] NOT NULL,
	[clientId] [varchar](50) NOT NULL,
	[doctorId] [varchar](50) NOT NULL,
	[diseaseId] [int] NULL,
	[documentName] [varchar](50) NULL,
	[path] [varchar](50) NULL,
 CONSTRAINT [PK_ClientDoc] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CryptoDbInfo]    Script Date: 5/16/2019 9:17:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CryptoDbInfo](
	[SchemaVersion] [int] NULL,
	[Launguage] [varchar](50) NULL,
	[Currency] [varchar](50) NULL,
	[DateTime] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Disease]    Script Date: 5/16/2019 9:17:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Disease](
	[id] [int] NULL,
	[Name] [varchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Doctors]    Script Date: 5/16/2019 9:17:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Doctors](
	[id] [int] NULL,
	[hospitaIId] [int] NULL,
	[doctorId] [varchar](50) NOT NULL,
	[firstName] [varchar](50) NULL,
	[middleName] [varchar](50) NULL,
	[lastName] [varchar](50) NULL,
	[gender] [int] NULL,
	[specialization] [varchar](max) NULL,
	[Address] [varchar](max) NULL,
	[city] [varchar](50) NULL,
	[state] [varchar](50) NULL,
	[country] [varchar](50) NULL,
	[mobileNo] [int] NULL,
	[emailId] [varchar](50) NULL,
	[createDate] [datetime] NULL,
	[modifyDate] [datetime] NULL,
	[modifyBy] [varchar](50) NULL,
 CONSTRAINT [PK_Doctors] PRIMARY KEY CLUSTERED 
(
	[doctorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Hospitals]    Script Date: 5/16/2019 9:17:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hospitals](
	[id] [int] NOT NULL,
	[name] [varchar](50) NULL,
	[Address] [varchar](max) NULL,
	[city] [varchar](50) NULL,
	[state] [varchar](50) NULL,
	[country] [varchar](50) NULL,
	[mobileNo] [int] NULL,
	[emailId] [varchar](50) NULL,
	[createDate] [datetime] NULL,
	[modifyDate] [datetime] NULL,
	[modifyBy] [varchar](50) NULL,
 CONSTRAINT [PK_Hospitals] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Spouse]    Script Date: 5/16/2019 9:17:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Spouse](
	[id] [int] NULL,
	[clientId] [varchar](50) NOT NULL,
	[firstName] [varchar](50) NULL,
	[middleName] [varchar](50) NULL,
	[lastName] [varchar](50) NULL,
	[gender] [int] NULL,
	[Address] [varchar](max) NULL,
	[city] [varchar](50) NULL,
	[state] [varchar](50) NULL,
	[country] [varchar](50) NULL,
	[mobileNo] [int] NULL,
	[emailId] [varchar](50) NULL,
	[createDate] [datetime] NULL,
	[modifyDate] [datetime] NULL,
	[modifyBy] [varchar](50) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[ClientDoc]  WITH CHECK ADD  CONSTRAINT [FK_ClientDoc_Client] FOREIGN KEY([clientId])
REFERENCES [dbo].[Client] ([clientId])
GO
ALTER TABLE [dbo].[ClientDoc] CHECK CONSTRAINT [FK_ClientDoc_Client]
GO
ALTER TABLE [dbo].[ClientDoc]  WITH CHECK ADD  CONSTRAINT [FK_ClientDoc_ClientDoc] FOREIGN KEY([id])
REFERENCES [dbo].[ClientDoc] ([id])
GO
ALTER TABLE [dbo].[ClientDoc] CHECK CONSTRAINT [FK_ClientDoc_ClientDoc]
GO
ALTER TABLE [dbo].[ClientDoc]  WITH CHECK ADD  CONSTRAINT [FK_ClientDoc_Doctors] FOREIGN KEY([doctorId])
REFERENCES [dbo].[Doctors] ([doctorId])
GO
ALTER TABLE [dbo].[ClientDoc] CHECK CONSTRAINT [FK_ClientDoc_Doctors]
GO
USE [master]
GO
ALTER DATABASE [NoorCare] SET  READ_WRITE 
GO
