import { StatusBar, StyleSheet, Text, View, Alert, Button, Pressable } from 'react-native';
import { WebView } from 'react-native-webview';
import { SafeAreaProvider, SafeAreaView } from 'react-native-safe-area-context';
import { useEffect, useRef } from 'react';

import * as Brightness from 'expo-brightness';

type MessageType = {
  key: string;
  value: string;
};


export default function App() {
  const webViewRef = useRef()
  const onMessage = (e: any) => {
    const data = e.nativeEvent.data // 로그인 하기
    try {
      const { key, value }: MessageType = JSON.parse(data);
      console.log(data);
      // Alert.alert(data);
      if (key == "brightness") {
        Brightness.setBrightnessAsync(value);
      }
    } catch {

    }

  }

  useEffect(() => {
    (async () => {
      const { status } = await Brightness.requestPermissionsAsync();
      if (status === 'granted') {
        Brightness.setSystemBrightnessAsync(1);
      }
    })();
  }, []);

  return (
    <SafeAreaProvider>
      <View style={{ flex: 1, marginTop: StatusBar.currentHeight }}>
        <Pressable onPress={() => {
          webViewRef.current.postMessage('업데이트 하세요');

        }}>
          <Text >버튼</Text>
        </Pressable>
      </View>

      <WebView
        ref={webViewRef}
        style={styles.container}
        source={{ uri: 'http://192.168.0.76:3000' }}
        onMessage={onMessage}

        originWhitelist={['*']}
        allowsInlineMediaPlayback
        javaScriptEnabled={true}
        scalesPageToFit
        mediaPlaybackRequiresUserAction={false}
        javaScriptEnabledAndroid
        useWebkit
        startInLoadingState={true}
        geolocationEnabled={true}
      />
    </SafeAreaProvider>
  )
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
    alignItems: 'center',
    justifyContent: 'center',
  },
});
