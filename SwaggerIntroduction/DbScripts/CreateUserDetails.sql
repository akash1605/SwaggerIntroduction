USE [ApiTest]
GO

/****** Object:  Table [dbo].[UserDetails]    Script Date: 11/26/2018 12:58:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserDetails](
    [UserDetailsId] [int] IDENTITY(1,1) NOT NULL,
    [FirstName] [varchar](255) NOT NULL,
    [LastName] [varchar](255) NULL,
    [CreationDate] [datetime] NOT NULL,
    [UserId] [int] NOT NULL,
PRIMARY KEY CLUSTERED
(
    [UserDetailsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserDetails]  WITH CHECK ADD  CONSTRAINT [FK_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[UserMaster] ([UserId])
GO

ALTER TABLE [dbo].[UserDetails] CHECK CONSTRAINT [FK_UserId]
GO