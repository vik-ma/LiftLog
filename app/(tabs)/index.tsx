import React, { useState } from "react";
import Button from "../../components/Button";
import { Text, View } from "../../components/Themed";

export default function TabOneScreen() {
  const [buttonText, setButtonText] = useState<string>("");

  return (
    <View className="flex justify-center items-center h-full">
      <View className="flex justify-center items-center">
        <Text>{buttonText}</Text>
        <Button
          label="Test Button"
          styling="bg-red-500"
          onPress={() => setButtonText("Test")}
        ></Button>
      </View>
    </View>
  );
}
