'use client';
import Image from 'next/image'
import styles from './page.module.css'
import { useEffect } from 'react';
import Link from 'next/link'

export default function Home() {
  return (
    <>
      <Link href="/test_brightness">
        밝기조절
      </Link>
    </>
  )
}
