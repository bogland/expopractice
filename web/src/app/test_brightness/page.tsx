'use client';
import Image from 'next/image'
import styles from './page.module.css'
import { useEffect } from 'react';

export default function Test_Brightness() {
  useEffect(() => {
    // ios
    window.addEventListener('message', (e) => alert(e.data));

    // android
    document.addEventListener('message', (e) => alert(e.data));
  }, [])
  return (
    <>
      <button onClick={() => {
        window.ReactNativeWebView.postMessage('로그인 하기')
      }}>버튼</button>
      <input type='range' min={0} max={1} step={0.2} onChange={(e) => {
        const data = {
          key: "brightness",
          value: e.target.value,
        };
        console.log(data);
        window.ReactNativeWebView.postMessage(JSON.stringify(data));
      }} />
    </>
  )
}
