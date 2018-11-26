USE [ApiTest]
GO

/****** Object:  Table [dbo].[UserAddress]    Script Date: 11/26/2018 12:54:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserAddress](
    [UserAddressId] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [int] NOT NULL,
    [AddressLineOne] [nvarchar](200) NOT NULL,
    [AddresslineTwo] [nvarchar](200) NULL,
    [City] [nvarchar](200) NULL,
    [State] [nvarchar](200) NULL,
    [Country] [nvarchar](200) NOT NULL,
    [Postcode] [nvarchar](100) NULL,
    [IsDefaultAddress] [bit] NOT NULL,
    [PhoneNumber] [int] NOT NULL,
PRIMARY KEY CLUSTERED
(
    [UserAddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserAddress]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[UserMaster] ([UserId])
GO