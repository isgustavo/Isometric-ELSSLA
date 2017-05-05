using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

[Serializable]
public class Coins {

	private const int INITIAL_COUNT = 10;
	private const int NORMAL_COST = 1;
	private const int FACEBOOK_LOGGIN_GIFT = 5;
	private const int DAILY_GIFT = 1;
	private const int VIDEO_GIFT = 5;

	private int _count;
	public int count {get { return _count; }}

	private DateTime _lastGift;
	public DateTime lastGift { get { return _lastGift;} }

	public Coins () {

		_count = INITIAL_COUNT;
		_lastGift = DateTime.Now;

	}


	/// <summary>
	/// Uses the coin. Every spawn/respawn cost a coin.
	/// </summary>
	public void UseCoin () {

		_count -= NORMAL_COST;
	}

	/// <summary>
	/// Sets the facebook gift. When player logged in to Facebook him receives a gift coins.
	/// </summary>
	public void SetFacebookGift() {

		_count += FACEBOOK_LOGGIN_GIFT;
	}

	/// <summary>
	/// Sets the daily gift. Each 24 hour the player receives a gift coins.
	/// </summary>
	public void SetDailyGift () {

		_count += DAILY_GIFT;
		_lastGift = DateTime.Now;
	}

	/// <summary>
	/// Sets the video gift. Each video watched on game the player receives a gift coins.
	/// </summary>
	public void SetVideoGift () {

		_count += VIDEO_GIFT;
	}
}
