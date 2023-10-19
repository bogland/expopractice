import type { Metadata } from 'next'
import { Inter } from 'next/font/google'
import "../css/reset.css"
import Link from 'next/link'
const inter = Inter({ subsets: ['latin'] })

export const metadata: Metadata = {
  title: 'Create Next App',
  description: 'Generated by create next app',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html>
      <body className={inter.className}>
        <Link href="/">
          <h1>홈<br /><br /><br /></h1>
        </Link>
        {children}</body>
    </html>
  )
}
